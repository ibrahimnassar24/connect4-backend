
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using connect4_backend.Data.DTOs;

namespace connect4_backend.Services;

public class Connect4GameManager : IConnect4GameManager
{

    private readonly ConcurrentDictionary<string, GameSession> _sessions;
    private readonly ConcurrentDictionary<string, string> _connectionIdToSessionMapping;
    private readonly ISharedService _sharedService;
    private readonly IGameHub _gameHub;

    public Connect4GameManager(
        ISharedService sharedService,
        IGameHub gameHub
    )
    {
        _sessions = new ConcurrentDictionary<string, GameSession>();
        _connectionIdToSessionMapping = new ConcurrentDictionary<string, string>();
        _sharedService = sharedService;
        _gameHub = gameHub;
    }

    public void AddGameSession(GameSession gameSession)
    {
        if (GameSessionExists(gameSession.MatchId))
            return;
        _sessions[gameSession.MatchId] = gameSession;
    }

    public bool RemoveGameSession(string id)
    {
        _sessions.Remove(id, out var session);
        return (session is null)
        ? false
        : true;

    }

    public GameSession? GetGameSession(string id)
    {
        if (GameSessionExists(id))
            return _sessions[id];
        return null;
    }

    public GameSession CreateGameSession(MatchDto match)
    {

        var gs = new GameSession(_gameHub)
        {
            MatchId = match.id,
            FirstPlayerEmail = match.firstPlayer,
            SecondPlayerEmail = match.secondPlayer,
            Turn = match.firstPlayer
        };

        // subscribe to the GameEnded event
        gs.GameEnded += GameEndedHandler;

        AddGameSession(gs);

        return gs;

    }

    public bool GameSessionExists(string id)
    {
        var temp = _sessions.ContainsKey(id);
        return temp;
    }

    public async void EndGameSession(GameSession session)
    {

        // update match info in database.
        await _sharedService.MarkMatchAsFinished(session.MatchId, session.Winner);


        RemoveFromConnectionIdToSessionMapping(session);

        session.Dispose();

        RemoveGameSession(session.MatchId);

    }

    public string? GetSessionIdFromConnectionId(string connectionId)
    {
        _connectionIdToSessionMapping.TryGetValue(connectionId, out var sessionId);
        return sessionId;

    }

    public void AddToConnectionIdToSessionMapping(GameSession session)
    {
        var sessionId = session.MatchId;
        var connectionId1 = session.FirstPlayerConnectionId;
        var connectionId2 = session.SecondPlayerConnectionId;

        _connectionIdToSessionMapping[connectionId1] = sessionId;
        _connectionIdToSessionMapping[connectionId2] = sessionId;
    }

    private void RemoveFromConnectionIdToSessionMapping(GameSession session)
    {
        var sessionId = session.MatchId;
        var connectionId1 = session!.FirstPlayerConnectionId;
        var connectionId2 = session.SecondPlayerConnectionId;

        _connectionIdToSessionMapping.TryRemove(connectionId1, out _);
        _connectionIdToSessionMapping.TryRemove(connectionId2, out _);
    }

    private void GameEndedHandler(object? sender, EventArgs e)
    {
        if (sender is null)
            return;

        var session = (GameSession)sender;
        EndGameSession(session);
    }
}