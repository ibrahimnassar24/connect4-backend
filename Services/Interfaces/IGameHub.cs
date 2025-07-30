
using connect4_backend.Data.DTOs;

namespace connect4_backend.Services;

public interface IGameHub
{
    public Task SendInvitationNotification(Invitation invitation);
    public Task SendInvitationAcceptanceNotification(MatchDto match);
    public Task DeclineInvitation(Invitation invitation);

    public Task SendMovement(Movement movement, string userId);
    public Task ChangeTurn(string turn, string id);
    public Task EndMatch(GameSession session);
}