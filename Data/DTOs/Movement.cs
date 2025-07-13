
using System.Text.Json;

namespace connect4_backend.Data.DTOs;

public class Movement
{
    public int matchId { get; set; }
    public string player { get; set; } = "";
    public int column { get; set; }
    public string format { get; set; } = "";

    public string ToJson() =>
    JsonSerializer.Serialize(this);
}