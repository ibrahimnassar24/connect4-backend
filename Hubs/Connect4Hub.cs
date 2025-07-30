using connect4_backend.Data.DTOs;
using connect4_backend.Data.Models;
using connect4_backend.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using System.Text.Json;

namespace connect4_backend.Hubs;

public class Connect4Hub : Hub
{

    private IConnect4GameManager _gameManager;
    private readonly IGameHub _gameHub;
    private readonly ISharedService _sharedService;

    public Connect4Hub(
IGameHub gameHub,
ISharedService sharedService,
        IConnect4GameManager gameManager)
    {
        _sharedService = sharedService;
        _gameManager = gameManager;
        _gameHub = gameHub;
    }



    public async Task Movements(Movement movement)
    {
        // get the game session for the match
        // and terminate if there is no session for the provided id
        var session = _gameManager.GetGameSession(movement.matchId);
        if (session is null)
            return;

        // check if the player who made the movement had made it in his turn
        // if not terminate
        if (!session.ValidateMovement(movement))
            return;

        // add to the specified column if there is empty place
        // if not terminate
        if (session.AddMovement(movement) is null)
            return;

        // give the other player his turn
        session.ChangeTurn();

        // send the movement to both player to update the state
        await _gameHub.SendMovement(movement, session.FirstPlayerId);
        await _gameHub.SendMovement(movement, session.SecondPlayerId);

        await _gameHub.ChangeTurn(session.Turn, session.FirstPlayerId);
        await _gameHub.ChangeTurn(session.Turn, session.SecondPlayerId);

        // check for the winner
        if (!session.ScanForWin())
            return;

        // notify who's won and who's lost
        await _gameHub.EndMatch(session);

        // update match information in database.
        await _sharedService.MarkMatchAsFinished(session.GameId, session.Winner);

        // remove the session from the game manager.
        _gameManager.RemoveGameSession(session.GameId);
    }

}