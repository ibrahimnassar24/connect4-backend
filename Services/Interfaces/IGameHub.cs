
using connect4_backend.Data.DTOs;

namespace connect4_backend.Services;

public interface IGameHub
{
    public Task SendInvitationNotification(Invitation invitation);
    public Task SendInvitationAcceptanceNotification(string userEmail, string invitationId, string matchId);
    public Task DeclineInvitation(Invitation invitation);

    public Task StartMatch(string connectionId, string matchId, MatchDto match);
    public Task SendMovement(string connectionId, string matchId, Movement movement);
    public Task SendSwitchTurnNotification(string connectionId, string matchId, string turn);
    public Task SendMatchWonNotification(string connectionId, string matchId);
    public Task SendMatchLostNotification(string connectionId, string matchId);
    public Task SendMatchForfittedNotification(string connectionId, string matchId);
    public Task SendMatchErrorNotification(string connectionId, string matchId, string message);
    public Task SendMatchWarningNotification(string connectionId, string matchId, string message);
}