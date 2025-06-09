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
    private readonly RedisShardManager _redisManager;

    public SummaryModel(ILogger<SummaryModel> logger, RedisShardManager redisManager)
    {
        _logger = logger;
        _redisManager = redisManager;
    }

    public string RankStr { get; set; }
    public string SimilarityStr { get; set; }

    public void OnGet(string id)
    {
        var mainDb = _redisManager.GetMainDatabase();
        string region = mainDb.StringGet(id);
        var shardDb = _redisManager.GetShardDatabase(region);

        string rankKey = "RANK-" + id;
        string similarityKey = "SIMILARITY-" + id;
        RankStr = shardDb.StringGet(rankKey);
        SimilarityStr = shardDb.StringGet(similarityKey);

        _logger.LogDebug(id);
    }
}
