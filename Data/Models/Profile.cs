using System;
using System.Collections.Generic;

namespace connect4_backend.Data.Models;

public partial class Profile
{
    public string Email { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string? Bio { get; set; }

    public string? AvatarUrl { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<Friend> FriendReceiverNavigations { get; set; } = new List<Friend>();

    public virtual ICollection<Friend> FriendSenderNavigations { get; set; } = new List<Friend>();

    public virtual ICollection<Match> MatchFirstPlayerNavigations { get; set; } = new List<Match>();

    public virtual ICollection<Match> MatchSecondPlayerNavigations { get; set; } = new List<Match>();

    public virtual ICollection<Match> MatchWinnerNavigations { get; set; } = new List<Match>();

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
}
