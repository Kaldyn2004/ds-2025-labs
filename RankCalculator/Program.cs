using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using StackExchange.Redis;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace Consumer;

class Program
{
    private const string QueueName = "valuator.processing.rank";
    private const string Host = "localhost";
    private const string User = "kirill";
    private const string Pass = "12345";

    // Конфигурация Redis
    private const string RedisMain = "localhost:6000";
    private const string RedisRU = "localhost:6001";
    private const string RedisFR = "localhost:6002";
    private const string RedisEU = "localhost:6003";
    private const string RedisUAE = "localhost:6004";
    private const string RedisASIA = "localhost:6005";

    private static ILogger<Program> _logger;

    public static async Task Main(string[] args)
    {
        Console.WriteLine("Rank calculator started");

        // Инициализация Redis Shard Manager
        var redisShardManager = new RedisShardManager(new Dictionary<string, string>
        {
            ["MAIN"] = RedisMain,
            ["RU"] = RedisRU,
            ["FR"] = RedisFR,
            ["EU"] = RedisEU,
            ["UAE"] = RedisUAE,
            ["ASIA"] = RedisASIA
        });

        ConnectionFactory factory = new ConnectionFactory()
        {
            HostName = Host,
            UserName = User,
            Password = Pass,
        };
        await using IConnection connection = await factory.CreateConnectionAsync();
        await using IChannel channel = await connection.CreateChannelAsync();

        await DeclareTopologyAsync(channel);
        string consumerTag = await RunConsumer(channel, redisShardManager);

        Console.WriteLine("Press Enter to exit");
        Console.ReadLine();

        await channel.BasicCancelAsync(consumerTag);

        Console.WriteLine("done");
    }

    private static async Task<string> RunConsumer(IChannel channel, RedisShardManager redisManager)
    {
        AsyncEventingBasicConsumer consumer = new(channel);
        consumer.ReceivedAsync += (_, eventArgs) => ConsumeAsync(channel, eventArgs, redisManager);

        return await channel.BasicConsumeAsync(
            queue: QueueName,
            autoAck: false,
            consumer: consumer
        );
    }

    private static async Task ConsumeAsync(IChannel channel, BasicDeliverEventArgs eventArgs, RedisShardManager redisManager)
    {

        string id = Encoding.UTF8.GetString(eventArgs.Body.ToArray());
        IDatabase mainDb = redisManager.GetMainDatabase();

        string region = mainDb.StringGet(id);
        string textKey = "TEXT-" + id;
        string rankKey = "RANK-" + id;
        Console.WriteLine($"LOOKUP: {id}, {region}");

        if (string.IsNullOrEmpty(region))
        {
            Console.WriteLine($"Region not found");
            return;
        }

        IDatabase shardDb = redisManager.GetShardDatabase(region);
        string text = shardDb.StringGet(textKey);

        // 3. Вычисляем и сохраняем ранг
        double rank = CalculateRank(text);
        shardDb.StringSet(rankKey, rank);

        shardDb.StringSet(rankKey, rank);

        var eventBody = new {
            EventType = "RankCalculated",
            TextId = id,
            Rank = rank,
            Region = region
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

public class RedisShardManager
{
    private readonly IConnectionMultiplexer _mainRedis;
    private readonly Dictionary<string, IConnectionMultiplexer> _shards;

    public RedisShardManager(Dictionary<string, string> configuration)
    {
        _mainRedis = ConnectionMultiplexer.Connect(configuration["MAIN"]);

        _shards = new Dictionary<string, IConnectionMultiplexer>
        {
            ["RU"] = ConnectionMultiplexer.Connect(configuration["RU"]),
            ["FR"] = ConnectionMultiplexer.Connect(configuration["FR"]),
            ["EU"] = ConnectionMultiplexer.Connect(configuration["EU"]),
            ["UAE"] = ConnectionMultiplexer.Connect(configuration["UAE"]),
            ["ASIA"] = ConnectionMultiplexer.Connect(configuration["ASIA"])
        };
    }

    public IDatabase GetMainDatabase() => _mainRedis.GetDatabase();

    public IDatabase GetShardDatabase(string regionCode)
    {
        if (_shards.TryGetValue(regionCode, out var redis))
            return redis.GetDatabase();

        throw new ArgumentException($"No shard found for region {regionCode}");
    }
}