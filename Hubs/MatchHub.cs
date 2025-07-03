using Microsoft.AspNetCore.SignalR;

namespace connect4_backend.Hubs;

public class MatchHub : Hub
{
    public async Task Notification(string user, string msg)
    {
        await Clients.All.SendAsync("listening", Context.User, msg);
    }
}