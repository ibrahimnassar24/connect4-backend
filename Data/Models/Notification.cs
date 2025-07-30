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


}
