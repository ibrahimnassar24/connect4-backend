
using System.Linq;
using System.Threading.Tasks;
using connect4_backend.Data.DTOs;

namespace connect4_backend.Services;

public class Connect4GameManager : IConnect4GameManager
{

    private readonly Dictionary<int, GameSession> _sessions;
    private readonly ISharedService _sharedService;

    public Connect4GameManager(
        ISharedService sharedService
    )
    {
        _sessions = new Dictionary<int, GameSession>();
        _sharedService = sharedService;
    }

    public void AddGameSession(GameSession gameSession)
    {
        if (GameSessionExists(gameSession.GameId))
            return;
        _sessions[gameSession.GameId] = gameSession;
    }

    public bool RemoveGameSession(int id)
    {
        return _sessions.Remove(id);
    }
    public GameSession? GetGameSession(int id)
    {
        if (GameSessionExists(id))
            return _sessions[id];
        return null;
    }

    public async Task<GameSession> CreateGameSession(MatchDto match)
    {
        var gs = new GameSession();
        gs.GameId = match.id;
        gs.FirstPlayerEmail = match.firstPlayer;
        gs.FirstPlayerId = await _sharedService.GetUserId(match.firstPlayer);
        gs.SecondPlayerEmail = match.secondPlayer;
        gs.Turn = match.firstPlayer;
        gs.SecondPlayerId = await _sharedService.GetUserId(match.secondPlayer);

        AddGameSession(gs);

        return gs;

    }

    public bool GameSessionExists(int id)
    {
        var temp = _sessions.ContainsKey(id);
        return temp;
    }
}