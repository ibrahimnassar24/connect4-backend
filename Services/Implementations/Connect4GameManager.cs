
using System.Linq;
using connect4_backend.Data.DTOs;

namespace connect4_backend.Services;

public class Connect4GameManager : IConnect4GameManager
{

    private readonly Dictionary<int, GameSession> _sessions;

    public Connect4GameManager()
    {
        _sessions = new Dictionary<int, GameSession>();
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

    public bool GameSessionExists(int id)
    {
        var temp = _sessions.ContainsKey(id);
        return temp;
    }
}