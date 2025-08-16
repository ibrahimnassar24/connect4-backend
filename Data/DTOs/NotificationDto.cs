using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;
using connect4_backend.Data.Models;

namespace connect4_backend.Data.DTOs;

public class NotificationDto
{
    public string id { get; set; } = "";

    public string? type { get; set; }

    public DateTime createdAt { get; set; }

    public string payload { get; set; } = "";

    public string ToJson() => JsonSerializer.Serialize(this);

    public static NotificationDto CreateGameInvitation(string payload)
=> new NotificationDto()
{
    id = Guid.NewGuid().ToString(),
    type = "invitation",
    createdAt = DateTime.UtcNow,
    payload = payload
};


    public static NotificationDto CreateInvitationAcceptance(string payload)
    => new NotificationDto()
    {
        id = Guid.NewGuid().ToString(),
        type = "invitationAccepted",
        payload = payload,
        createdAt = DateTime.UtcNow
    };

    public static NotificationDto CreateInvitationDecline(string payload)
    => new NotificationDto()
    {
        id = Guid.NewGuid().ToString(),
        type = "invitationDeclined",
        payload = payload,
        createdAt = DateTime.UtcNow
    };

    public static NotificationDto CreateStartMatchNotification(string payload)
    => new NotificationDto()
    {
        id = Guid.NewGuid().ToString(),
        type = "startMatch",
        payload = payload,
        createdAt = DateTime.UtcNow
    };

    public static NotificationDto CreateSwitchturnNotification(string payload) =>
    new NotificationDto()
    {
        id = Guid.NewGuid().ToString(),
        type = "switchTurn",
        payload = payload,
        createdAt = DateTime.UtcNow
    };

    public static NotificationDto CreateAddMovementNotification(Movement movement) =>
    new NotificationDto()
    {
        id = Guid.NewGuid().ToString(),
        type = "addMovement",
        payload = movement.ToJson(),
        createdAt = DateTime.UtcNow
    };

    public static NotificationDto CreateMatchWonNotification()
    => new NotificationDto()
    {
        id = Guid.NewGuid().ToString(),
        type = "matchWon",
        payload = "",
        createdAt = DateTime.UtcNow
    };

    public static NotificationDto CreateMatchLostNotification()
    => new NotificationDto()
    {
        id = Guid.NewGuid().ToString(),
        type = "matchLost",
        payload = "",
        createdAt = DateTime.UtcNow
    };

    public static NotificationDto CreateMatchForfittedNotification()
     => new NotificationDto()
     {
         id = Guid.NewGuid().ToString(),
         type = "matchForfitted",
         payload = "",
         createdAt = DateTime.UtcNow
     };

    public static NotificationDto CreateMatchErrorNotification(string payload)
     => new NotificationDto()
     {
         id = Guid.NewGuid().ToString(),
         type = "matchError",
         payload = payload,
         createdAt = DateTime.UtcNow
     };

    public static NotificationDto CreateMatchWarningNotification(string payload)
     => new NotificationDto()
     {
         id = Guid.NewGuid().ToString(),
         type = "matchWarning",
         payload = payload,
         createdAt = DateTime.UtcNow
     };
}