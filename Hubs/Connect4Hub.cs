using connect4_backend.Data.DTOs;
using connect4_backend.Data.Models;
using connect4_backend.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using System.Text.Json;

namespace connect4_backend.Hubs;

public class Connect4Hub : Hub
{

    private UserManager<IdentityUser> _userManager;
    private IConnect4GameManager _gameManager;
    private readonly ILogger<Connect4Hub> _logger;
    public Connect4Hub(
        UserManager<IdentityUser> userManager,
        ILogger<Connect4Hub> logger,
        IConnect4GameManager gameManager)
    {
        _userManager = userManager;
        _gameManager = gameManager;
        _logger = logger;
    }

    public async Task SendNotificationAsync(Notification notification)
    {
        var receiver = await _userManager.FindByEmailAsync(notification.Receiver);
        var receiverId = receiver.Id;
        var json = JsonSerializer.Serialize(notification);
        await Clients.User(receiverId).SendAsync("listening", json);
    }

    public async Task Movements(Movement movement)
    {
        Console.WriteLine($"matchId: {movement.matchId}");
        // get the game session for the match
        // and terminate if there is no session for the provided id
        var session = _gameManager.GetGameSession(movement.matchId);
        Console.WriteLine($"session is null {session is null}");
        if (session is null)
            return;

        // check if the player who made the movement had made it in his turn
        // if not terminate
        Console.WriteLine($"turn: {session.Turn}");
        Console.WriteLine($"player: {movement.player}");
        if (!session.ValidateMovement(movement))
            return;

        // add to the specified column if there is empty place
        // if not terminate
        if (session.AddMovement(movement) is null)
            return;
        Console.WriteLine("the movement was added successfully");
        // give the other player his turn
        session.ChangeTurn();
        Console.WriteLine($"p1 id: {session.FirstPlayerId}");
        Console.WriteLine($"p2 id: {session.SecondPlayerId}");
        // send the movement to both player to update the state
        var data = movement.ToJson();
        await Clients.User(session.FirstPlayerId).SendAsync("movement", data);
        await Clients.User(session.FirstPlayerId).SendAsync("changeTurn", session.Turn);
        await Clients.User(session.SecondPlayerId).SendAsync("movement", data);
        await Clients.User(session.SecondPlayerId).SendAsync("changeTurn", session.Turn);

        // check for the winner
        if (!session.ScanForWin())
            return;
        await Clients.User(session.FirstPlayerId).SendAsync("matchFinished", session.Winner);
        await Clients.User(session.SecondPlayerId).SendAsync("matchFinished", session.Winner);
    }

}