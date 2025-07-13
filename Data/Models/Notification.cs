using System;
using System.Collections.Generic;
using System.Text.Json;

namespace connect4_backend.Data.Models;

public partial class Notification
{
    public int Id { get; set; }

    public string Receiver { get; set; } = null!;

    public string? Message { get; set; }

    public DateTime CreatedAt { get; set; }

    public string? Type { get; set; }

    public string? Link { get; set; }

    public virtual Profile ReceiverNavigation { get; set; } = null!;

    public static Notification CreateGameInvitation(string name)
    => new Notification()
    {
        Type = "GameInvitation",
        CreatedAt = DateTime.UtcNow,
        Message = $"{name} invites you to a game"

    };

    public string ToJson()
    => JsonSerializer.Serialize(this);

    public static Notification CreateInvitationAcceptance(string name)
    => new Notification()
    {
        Message = $"{name} had accepted your invitation",
        Type = "InvitationAcceptance",
        CreatedAt = DateTime.UtcNow
    };

    public static Notification CreateInvitationDecline(string name)
    => new Notification()
    {
        Message = $"{name} has declined your invitation",
        Type = "InvitationDecline",
CreatedAt = DateTime.UtcNow
    };
}
