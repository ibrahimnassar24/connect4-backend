using connect4_backend.Data.DTOs;

namespace connect4_backend.Services;

public interface IConnect4GameManager
{
    void AddGameSession(GameSession gameSession);
    bool RemoveGameSession(string id);
    GameSession? GetGameSession(string id);
    GameSession CreateGameSession(MatchDto match);
    void EndGameSession(GameSession session);
    bool GameSessionExists(string id);

    public void AddToConnectionIdToSessionMapping(GameSession session);
    public string? GetSessionIdFromConnectionId(string connectionId);
}