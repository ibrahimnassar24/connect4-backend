
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace connect4_backend.Data.DTOs;

public class InvitationRequest
{
    public string email { get; set; } = "";
}