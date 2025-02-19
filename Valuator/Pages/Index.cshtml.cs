using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StackExchange.Redis;
using System;

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
            // Сохраняем текст в Redis
            _redisDatabase.StringSet(textKey, text);

            string rankKey = "RANK-" + id;
            // Вычисляем ранг (пример: просто присваиваем значение 1, в реальности это может быть сложнее)
            int rank = 1; // TODO: Замените на реальную логику вычисления ранга
            _redisDatabase.StringSet(rankKey, rank);

            string similarityKey = "SIMILARITY-" + id;
            // Вычисляем сходство (пример: просто присваиваем значение 0.5, в реальности это может быть сложнее)
            double similarity = 0.5; // TODO: Замените на реальную логику вычисления сходства
            _redisDatabase.StringSet(similarityKey, similarity, TimeSpan.FromHours(24)); // Например, срок хранения 24 часа

            return Redirect($"summary?id={id}");
        }
}
