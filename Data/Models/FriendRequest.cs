using System;
using System.Collections.Generic;

namespace connect4_backend.Data.Models;

public partial class FriendRequest
{
    public string Sender { get; set; } = null!;

    public string Receiver { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime? ClosedAt { get; set; }

    public string? Status { get; set; }

    public virtual Profile ReceiverNavigation { get; set; } = null!;

    public virtual Profile SenderNavigation { get; set; } = null!;
}
