
using System;
using System.Collections.Generic;
using System.Text.Json;
using connect4_backend.Data.Models;

namespace connect4_backend.Data.DTOs;

public class MatchDto
{
    public MatchDto()
    {
        id = Guid.NewGuid().ToString();
    }

    public MatchDto(Match match)
    {
        this.id = match.Id;
        this.firstPlayer = match.FirstPlayer;
        this.secondPlayer = match.SecondPlayer;
        this.status = match.Status;
        this.createdAt = match.CreatedAt;
        this.updatedAt = match.UpdatedAt;
        this.winner = match.Winner;
        this.turn = match.FirstPlayer;
    }

    public string id { get; set; }
    public string firstPlayer { get; set; } = null!;
    public string secondPlayer { get; set; } = null!;
    public string? winner { get; set; }
    public DateTime createdAt { get; set; }
    public DateTime updatedAt { get; set; }
    public string status { get; set; } = null!;
    public string invitationId { get; set; }
    public string? turn { get; set; } = null;
    public string ToJson()
    {
        return JsonSerializer.Serialize(this);
    }

}
