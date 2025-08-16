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

    public async Task SendInvitationAcceptanceNotification(string userEmail, string invitationId, string matchId)
    {
        // get user id.
        var id = await _sharedService.GetUserId(userEmail);
        if (id is null)
            return;

        // create the payload
        var payloadObj = new
        {
            matchId = matchId
        };
        var payload = JsonSerializer.Serialize(payloadObj);
        var notification = NotificationDto.CreateInvitationAcceptance(payload);
        var data = notification.ToJson();

        await _hub.Clients.User(id)
        .SendAsync("invitation", invitationId, data);
    }

    public async Task DeclineInvitation(Invitation invitation)
    {
        var id = await _sharedService.GetUserId(invitation.senderEmail);
        if (id is null)
            return;
        var notification = NotificationDto.CreateInvitationDecline("");
        var data = notification.ToJson();
        await _hub.Clients.User(id)
            .SendAsync("invitation", invitation.invitationId, data);
    }

    public async Task StartMatch(string connectionId, string matchId, MatchDto match)
    {
        var payload = JsonSerializer.Serialize(match);
        var notification = NotificationDto.CreateStartMatchNotification(payload);
        var data = notification.ToJson();

        await _hub.Clients.Client(connectionId).SendAsync("match", matchId, data);

    }

    public async Task SendMovement(string connectionId, string matchId, Movement movement)
    {
        var notification = NotificationDto.CreateAddMovementNotification(movement);
        var data = notification.ToJson();
        await _hub.Clients.Client(connectionId).SendAsync("match", matchId, data);
    }

    public async Task SendSwitchTurnNotification(string connectionId, string matchId, string turn)
    {
        var payloadObj = new
        {
            turn = turn
        };
        var payloadData = JsonSerializer.Serialize(payloadObj);
        var notification = NotificationDto.CreateSwitchturnNotification(payloadData);
        var data = notification.ToJson();
        await _hub.Clients.Client(connectionId).SendAsync("match", matchId, data);
    }

    public async Task SendMatchWonNotification(string connectionId, string matchId)
    {
        var notification = NotificationDto.CreateMatchWonNotification();
        var data = notification.ToJson();
        await _hub.Clients.Client(connectionId).SendAsync("match", matchId, data);
    }

    public async Task SendMatchLostNotification(string connectionId, string matchId)
    {
        var notification = NotificationDto.CreateMatchLostNotification();
        var data = notification.ToJson();
        await _hub.Clients.Client(connectionId).SendAsync("match", matchId, data);
    }

    public async Task SendMatchForfittedNotification(string connectionId, string matchId)
    {
        var notification = NotificationDto.CreateMatchForfittedNotification();
        var data = notification.ToJson();

        await _hub.Clients.Client(connectionId).SendAsync("match", matchId, data);
    }

    public async Task SendMatchErrorNotification(string connectionId, string matchId, string message)
    {
        var payloadObj = new
        {
            msg = message
        };
        var payloadData = JsonSerializer.Serialize(payloadObj);
        var notification = NotificationDto.CreateMatchErrorNotification(payloadData);
        var data = notification.ToJson();

        await _hub.Clients.Client(connectionId).SendAsync("match", matchId, data);
    }


    public async Task SendMatchWarningNotification(string connectionId, string matchId, string message)
    {
        var payloadObj = new
        {
            msg = message
        };
        var payloadData = JsonSerializer.Serialize(payloadObj);
        var notification = NotificationDto.CreateMatchWarningNotification(payloadData);
        var data = notification.ToJson();

        await _hub.Clients.Client(connectionId).SendAsync("match", matchId, data);
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