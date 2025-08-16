using connect4_backend.Data;
using connect4_backend.Data.DTOs;
using connect4_backend.Data.Models;
using connect4_backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using System.Security.Claims;
using System.Text.Json;

namespace connect4_backend.Hubs;

public class Connect4Hub : Hub
{

    private IConnect4GameManager _gameManager;
    private readonly IGameHub _gameHub;
    private readonly Connect4Context _context;
    private readonly ISharedService _sharedService;

    public Connect4Hub(
IGameHub gameHub,
ISharedService sharedService,
        IConnect4GameManager gameManager,
        Connect4Context context)
    {
        _sharedService = sharedService;
        _gameManager = gameManager;
        _gameHub = gameHub;
        _context = context;
    }

    [Authorize]
    public async Task JoinMatch(string matchId)
    {

        var connectionId = Context.ConnectionId;
        var session = _gameManager.GetGameSession(matchId);
        if (session is null)
        {
            await _gameHub.SendMatchErrorNotification(connectionId, matchId, "no session has been created for this match"); ;
            return;
        }

        var email = Context.User.FindFirstValue(ClaimTypes.Email);
        if (session.FirstPlayerEmail == email)
        {
            if (String.IsNullOrEmpty(session.FirstPlayerConnectionId))
            {
                session.FirstPlayerConnectionId = connectionId;
            }
            else
            {
                await _gameHub.SendMatchErrorNotification(connectionId, matchId, "you have already joined from another device");
            }
        }
        else if (session.SecondPlayerEmail == email)
        {
            if (string.IsNullOrEmpty(session.SecondPlayerConnectionId))
            {
                session.SecondPlayerConnectionId = connectionId;
            }
            else
            {
                await _gameHub.SendMatchErrorNotification(connectionId, matchId, "you have already joined from another device");
            }
        }
        if (String.IsNullOrEmpty(session.FirstPlayerConnectionId) ||
        string.IsNullOrEmpty(session.SecondPlayerConnectionId))
            return;
        _gameManager.AddToConnectionIdToSessionMapping(session);
        // get match from database
        var match = await _context.Matches.FindAsync(matchId);
        if (match is null)
            return;

        // notify players with match data
        var dto = new MatchDto(match);
        await _gameHub.StartMatch(session.FirstPlayerConnectionId, matchId, dto);
        await _gameHub.StartMatch(session.SecondPlayerConnectionId, matchId, dto);
    }


    [Authorize]
    public async Task Movements(Movement movement)
    {
        var connectionId = Context.ConnectionId;
        var matchId = movement.matchId;

        // get the game session for the match
        // and terminate if there is no session for the provided id
        var session = _gameManager.GetGameSession(movement.matchId);
        if (session is null)
        {
            await _gameHub.SendMatchErrorNotification(connectionId, matchId, "no session is associated with the provided match id");
            return;
        }
        // register the movement by
        // checking if the player who made the movement had made it in his turn
        // adding the movement to the specified column
        // notify the players to update the state
        // terminate if any of the conditions isn't met
        var res = await session.RegisterMovement(movement);
        if (!res) return;

        // give the other player his turn
        // and send notification to notify both players
        await session.SwitchTurn();

        // check for the winner
        if (!session.ScanForWin())
            return;

        // notify who's won and who's lost
        await session.GameOver();

    }

    public override async Task OnConnectedAsync()
    {

    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var connectionId = Context.ConnectionId;
        var sessionId = _gameManager.GetSessionIdFromConnectionId(connectionId);

        if (String.IsNullOrEmpty(sessionId))
            return;

        var session = _gameManager.GetGameSession(sessionId);
        if (session is null)
            return;

        await session.Forfit(connectionId);

    }


}