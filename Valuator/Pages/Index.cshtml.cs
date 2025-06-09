using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StackExchange.Redis;
using System;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Linq;
using RabbitMQ.Client;

namespace Valuator.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly IDatabase _redisDatabase;
    private readonly IModel _rabbitmqChannel;

    public IndexModel(ILogger<IndexModel> logger, IConnectionMultiplexer redisConnection, IModel rabbitmqChannel)
    {
        _logger = logger;
        _redisDatabase = redisConnection.GetDatabase();
        _rabbitmqChannel = rabbitmqChannel;
    }

    public void OnGet()
    {
    }

    public IActionResult OnPost(string text, string region)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return Redirect($"/");
        }
        _logger.LogDebug(text);

        string id = Guid.NewGuid().ToString();

        string textKey = "TEXT-" + id;
        _redisDatabase.StringSet(textKey, text);

        double similarity = CheckSimilarity(text);
        _redisDatabase.StringSet(text, "1");

        var body = Encoding.UTF8.GetBytes(id);
        _rabbitmqChannel.BasicPublish(exchange: "",
                                     routingKey: "valuator.processing.rank",
                                     basicProperties: null,
                                     body: body);

        string similarityKey = "SIMILARITY-" + id;

        _redisDatabase.StringSet(similarityKey, similarity);

         var eventBody = new {
            EventType = "SimilarityCalculated",
            TextId = id,
            Similarity = similarity,
        };
        var eventBytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(eventBody));

         _rabbitmqChannel.BasicPublish(
             exchange: "logs",
             routingKey: "",
             body: eventBytes
         );

        return Redirect($"summary?id={id}");
    }

    private double CheckSimilarity(string text)
    {
        bool keyExists = _redisDatabase.KeyExists(text);
        if (keyExists)
        {
            return 1;
        }
        return 0.0;
    }
}
