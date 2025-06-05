using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace Valuator.Pages;
public class SummaryModel : PageModel
{
    private readonly ILogger<SummaryModel> _logger;
    private readonly IDatabase _redisDatabase;

    public SummaryModel(ILogger<SummaryModel> logger, IConnectionMultiplexer redisConnection)
    {
        _logger = logger;
        _redisDatabase = redisConnection.GetDatabase();
    }

    public string RankStr { get; set; }
    public string SimilarityStr { get; set; }
    public string Id { get; set; }

    public void OnGet(string id)
    {
        string rankKey = "RANK-" + id;
        string similarityKey = "SIMILARITY-" + id;
        RankStr = _redisDatabase.StringGet(rankKey);
        SimilarityStr = _redisDatabase.StringGet(similarityKey);
        Id = id;

        _logger.LogDebug(id);
    }
}
