
using NuGet.Common;

namespace connect4_backend.Data.DTOs;

public class Invitation
{
    public string invitationId { get; }
    public string senderEmail { get; set; }
    public string receiverEmail { get; set; }
    public DateTime expiresAt { get; }

    public Invitation(string sender, string receiver)
    {
        invitationId = Guid.NewGuid().ToString();
        expiresAt = DateTime.UtcNow.AddMinutes(5);
        senderEmail = sender;
        receiverEmail = receiver;
    }

    public bool HasExpired()
    {
        if (DateTime.UtcNow > expiresAt)
            return true;

        return false;
    }
}