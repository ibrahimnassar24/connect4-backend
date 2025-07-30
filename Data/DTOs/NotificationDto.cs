using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;
namespace connect4_backend.Data.DTOs;

public class NotificationDto
{
    public string id { get; set; } = "";

    public string? type { get; set; }

    public DateTime createdAt { get; set; }

    public string payload { get; set; }


    public static NotificationDto CreateGameInvitation(string payload)
=> new NotificationDto()
{
    id = Guid.NewGuid().ToString(),
    type = "GameInvitation",
    createdAt = DateTime.UtcNow,
    payload = payload
};

    public string ToJson()
    => JsonSerializer.Serialize(this);

    public static NotificationDto CreateInvitationAcceptance(string payload)
    => new NotificationDto()
    {
        id = Guid.NewGuid().ToString(),
        type = "InvitationAcceptance",
        payload = payload,
        createdAt = DateTime.UtcNow
    };

    public static NotificationDto CreateInvitationDecline(string payload)
    => new NotificationDto()
    {
        id = Guid.NewGuid().ToString(),
        type = "InvitationDecline",
        payload = payload,
        createdAt = DateTime.UtcNow
    };
}