using connect4_backend.Data;
using connect4_backend.Data.DTOs;
using connect4_backend.Hubs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace connect4_backend.Services;

public class GameHub : IGameHub
{

    private readonly IHubContext<Connect4Hub> _hub;
    private readonly ISharedService _sharedService;
    private readonly IServiceProvider _serviceProvider;

    public GameHub(
        IHubContext<Connect4Hub> hub,
        IServiceProvider serviceProvider,
        ISharedService sharedService
        )
    {
        _hub = hub;
        _serviceProvider = serviceProvider;
        _sharedService = sharedService;
    }

    public async Task SendInvitationNotification(Invitation invitation)
    {
        // get the id of the receiver
        // by retrieving the user object for the receiver
        var receiverId = await _sharedService.GetUserId(invitation.receiverEmail);

        if (receiverId is null)
            return;
        // get the profile of the receiver to get his name

        var name = await GetUserFullName(invitation.senderEmail);

        if (name is null)
            return;

        // create the game invitation notification
        // and convert it into json string
        var payloadObj = new
        {
            msg = $"{name} is inviting you to a game",
            invitationId = invitation.invitationId
        };
        var payload = JsonSerializer.Serialize(payloadObj);
        var notification = NotificationDto.CreateGameInvitation(payload);
        var data = notification.ToJson();

        // send a notification to the receiver
        await _hub.Clients.User(receiverId)
            .SendAsync("notification", data);
    }

    public async Task SendInvitationAcceptanceNotification(MatchDto match)
    {
        // get user id.
        var id = await _sharedService.GetUserId(match.firstPlayer);
        if (id is null)
            return;

        // create the payload
            var data = JsonSerializer.Serialize(match);

        await _hub.Clients.User(id)
        .SendAsync("match", data);
    }

    public async Task DeclineInvitation(Invitation invitation)
    {
        var id = await _sharedService.GetUserId(invitation.senderEmail);

        await _hub.Clients.User(id)
        .SendAsync("cancel", invitation.invitationId);
    }

    public async Task SendMovement(Movement movement, string userId)
    {
        var data = movement.ToJson();
        await _hub.Clients.User(userId).SendAsync("movement", data);
    }

    public async Task ChangeTurn(string turn, string id)
    {
        await _hub.Clients.User(id).SendAsync("changeTurn", turn);
    }

    public async Task EndMatch(GameSession session)
    {
        var winnerId = await _sharedService.GetUserId(session.Winner);

        if (winnerId == session.FirstPlayerId)
        {
            await _hub.Clients.User(session.FirstPlayerId).SendAsync("gameWon");
            await _hub.Clients.User(session.SecondPlayerId).SendAsync("gameOver");
        }
        else if( winnerId == session.SecondPlayerId)
        {
            await _hub.Clients.User(session.SecondPlayerId).SendAsync("gameWon");
            await _hub.Clients.User(session.FirstPlayerId).SendAsync("gameOver");
        }
    }

    
    private async Task<string?> GetUserFullName(string email)
    {
        using var scope = _serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<Connect4Context>();

        var profile = await db.Profiles.FindAsync(email);

        return (profile is not null)
        ? $"{profile.FirstName} {profile.LastName}"
        : null;
    }
}