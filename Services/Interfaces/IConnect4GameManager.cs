using connect4_backend.Data.DTOs;

namespace connect4_backend.Services;

public interface IConnect4GameManager
{
    void AddGameSession(GameSession gameSession);
    bool RemoveGameSession(int id);
    GameSession? GetGameSession(int id);
    Task<GameSession> CreateGameSession(MatchDto match);
    bool GameSessionExists(int id);
}