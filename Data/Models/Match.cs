using System;
using System.Collections.Generic;

namespace connect4_backend.Data.Models;

public partial class Match
{
    public int Id { get; set; }

    public string FirstPlayer { get; set; } = null!;

    public string SecondPlayer { get; set; } = null!;

    public string? Winner { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public string Status { get; set; } = null!;

    public virtual Profile FirstPlayerNavigation { get; set; } = null!;

    public virtual Profile SecondPlayerNavigation { get; set; } = null!;

    public virtual Profile? WinnerNavigation { get; set; }
}
