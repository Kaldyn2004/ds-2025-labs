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
        if (string.IsNullOrEmpty(text))
        {
            return 0.0;
        }
        var alphabetChars = Regex.Replace(text, @"[A-Za-zА-Яа-я]", "");

        double rank = (double)alphabetChars.Length / text.Length;

        return Math.Min(Math.Max(rank, 0.0), 1.0);
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
