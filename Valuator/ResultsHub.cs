using Microsoft.AspNetCore.SignalR;

namespace Valuator.Hubs;

public class ResultsHub : Hub
{
    public async Task SubscribeToResults(string id)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, id);
    }

     // Этот метод будет вызываться из RankCalculator
     public async Task NotifyRankCalculated(string textId, double rank)
     {
         await Clients.Group(textId).SendAsync("ReceiveRankUpdate", rank);
     }
}