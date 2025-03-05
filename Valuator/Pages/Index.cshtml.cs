using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StackExchange.Redis;
using System;
using System.Text.RegularExpressions;
using System.Linq;

namespace Valuator.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly IDatabase _redisDatabase;

    public IndexModel(ILogger<IndexModel> logger, IConnectionMultiplexer redisConnection)
    {
        _logger = logger;
        _redisDatabase = redisConnection.GetDatabase();
    }

    public void OnGet()
    {

    }

    public IActionResult OnPost(string text)
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

        string rankKey = "RANK-" + id;
        double rank = CalculateRank(text);
        _redisDatabase.StringSet(rankKey, rank);

        string similarityKey = "SIMILARITY-" + id;

        _redisDatabase.StringSet(similarityKey, similarity);

        return Redirect($"summary?id={id}");
    }

    private double CalculateRank(string text)
    {
        return text.Count(ch => !char.IsLetter(ch)) / (double)text.Length;
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
