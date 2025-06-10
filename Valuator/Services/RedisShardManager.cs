using Microsoft.Extensions.Configuration;
using StackExchange.Redis;

namespace Valuator;

public class RedisShardManager
{
    private readonly IConnectionMultiplexer _mainConnection;
    private readonly Dictionary<string, IConnectionMultiplexer> _shardConnections;

    public RedisShardManager(IConfiguration configuration)
    {
        _mainConnection = CreateConnection(configuration["MAIN"]);

        _shardConnections = new Dictionary<string, IConnectionMultiplexer>
        {
            ["RU"] = CreateConnection(configuration["RU"]),
            ["FR"] = CreateConnection(configuration["EU"]),
            ["EU"] = CreateConnection(configuration["EU"]),
            ["UAE"] = CreateConnection(configuration["ASIA"]),
            ["ASIA"] = CreateConnection(configuration["ASIA"])
        };
    }

    private IConnectionMultiplexer CreateConnection(string connectionString)
    {
        return ConnectionMultiplexer.Connect(new ConfigurationOptions
        {
            EndPoints = { connectionString },
            AbortOnConnectFail = false,
            ConnectRetry = 5,
            ConnectTimeout = 5000
        });
    }

    public IDatabase GetMainDatabase() => _mainConnection.GetDatabase();

    public IDatabase GetShardDatabase(string regionCode)
    {
        if (_shardConnections.TryGetValue(regionCode, out var connection))
        {
            return connection.GetDatabase();
        }
        throw new ArgumentException($"No Redis shard configured for region {regionCode}");
    }

    public IEnumerable<string> GetAvailableRegions() => _shardConnections.Keys;
}