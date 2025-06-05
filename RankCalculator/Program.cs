using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using StackExchange.Redis;
using System.Text.Json;
using Microsoft.AspNetCore.SignalR.Client;

namespace Consumer;

class Program
{
    private const string QueueName = "valuator.processing.rank";
    private const string Host = "localhost";
    private const string User = "kirill";
    private const string Pass = "12345";
    private const string RedisConnectionString = "localhost:6379";

    public static async Task Main(string[] args)
    {
        Console.WriteLine("Rank calculator started");

        using var redis = ConnectionMultiplexer.Connect(RedisConnectionString);
        var db = redis.GetDatabase();

        ConnectionFactory factory = new ConnectionFactory()
        {
            HostName = Host,
            UserName = User,
            Password = Pass,
        };

         var hubConnection = new HubConnectionBuilder()
                .WithUrl("http://localhost:5001/resultsHub")  // URL хаба Valuator
                .WithAutomaticReconnect(new[] { TimeSpan.Zero, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(5) })
                .Build();

            hubConnection.Closed += async (error) =>
            {
                Console.WriteLine($"SignalR connection closed: {error?.Message}");
                await Task.Delay(5000);
                await hubConnection.StartAsync();
            };

         try
         {
             await hubConnection.StartAsync();
             Console.WriteLine("Connected to SignalR Hub");
         }
         catch (Exception ex)
         {
             Console.WriteLine($"SignalR connection error: {ex.Message}");
         }

        await using IConnection connection = await factory.CreateConnectionAsync();
        await using IChannel channel = await connection.CreateChannelAsync();

        await DeclareTopologyAsync(channel);
        string consumerTag = await RunConsumer(channel, db, hubConnection);

        Console.WriteLine("Press Enter to exit");
        Console.ReadLine();

        await channel.BasicCancelAsync(consumerTag);

        Console.WriteLine("done");
    }

    private static async Task<string> RunConsumer(IChannel channel, IDatabase db, HubConnection hubConnection)
    {
        AsyncEventingBasicConsumer consumer = new(channel);
        consumer.ReceivedAsync += (_, eventArgs) => ConsumeAsync(channel, eventArgs, db, hubConnection);
        return await channel.BasicConsumeAsync(
            queue: QueueName,
            autoAck: false,
            consumer: consumer
        );
    }

    private static async Task ConsumeAsync(IChannel channel, BasicDeliverEventArgs eventArgs, IDatabase db, HubConnection hubConnection)
    {
        string id = Encoding.UTF8.GetString(eventArgs.Body.ToArray());

        TimeSpan interval = TimeSpan.FromSeconds(new Random().Next(3, 15));
        Console.WriteLine($"Waiting {interval}");
        await Task.Delay(interval);

        string textKey = "TEXT-" + id;
        string rankKey = "RANK-" + id;
        string text = db.StringGet(textKey);
        double rank = CalculateRank(text);

        db.StringSet(rankKey, rank);

        try
        {
            await hubConnection.InvokeAsync("NotifyRankCalculated", id, rank);
            Console.WriteLine($"Результат для {id} отправлен через SignalR: {rank:P2}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка SignalR: {ex.Message}");
        }

        var eventBody = new {
            EventType = "RankCalculated",
            TextId = id,
            Rank = rank,
        };
        var eventBytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(eventBody));

        await channel.BasicPublishAsync(
            exchange: "logs",
            routingKey: string.Empty,
            body: eventBytes
        );

        Console.WriteLine($"Consuming: {id} from subject");
        await channel.BasicAckAsync(eventArgs.DeliveryTag, false);
    }

     private static double CalculateRank(string text)
     {
         return text.Count(ch => !char.IsLetter(ch)) / (double)text.Length;
     }

    /// <summary>
    ///  Определяет топологию: queue -> consumer.
    /// </summary>
    private static async Task DeclareTopologyAsync(IChannel channel)
    {
        await channel.QueueDeclareAsync(
            queue: QueueName,
            durable: true,
            exclusive: false,
            autoDelete: false
        );
    }
}