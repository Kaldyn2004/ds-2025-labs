using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using StackExchange.Redis;

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
        await using IConnection connection = await factory.CreateConnectionAsync();
        await using IChannel channel = await connection.CreateChannelAsync();

        await DeclareTopologyAsync(channel);
        string consumerTag = await RunConsumer(channel, db);

        Console.WriteLine("Press Enter to exit");
        Console.ReadLine();

        await channel.BasicCancelAsync(consumerTag);

        Console.WriteLine("done");
    }

    private static async Task<string> RunConsumer(IChannel channel, IDatabase db)
    {
        AsyncEventingBasicConsumer consumer = new(channel);
        consumer.ReceivedAsync += (_, eventArgs) => ConsumeAsync(channel, eventArgs, db);
        return await channel.BasicConsumeAsync(
            queue: QueueName,
            autoAck: false,
            consumer: consumer
        );
    }

    private static async Task ConsumeAsync(IChannel channel, BasicDeliverEventArgs eventArgs, IDatabase db)
    {
        string id = Encoding.UTF8.GetString(eventArgs.Body.ToArray());
        string textKey = "TEXT-" + id;
        string rankKey = "RANK-" + id;
        string text = db.StringGet(textKey);
        double rank = CalculateRank(text);

        db.StringSet(rankKey, rank);

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