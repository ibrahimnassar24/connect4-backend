using connect4_backend.Data.DTOs;
using connect4_backend.Hubs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;

namespace connect4_backend.Services;

public class Connect4InvitationManager : IConnect4InvitationManager
{
    private readonly Dictionary<string, Invitation> _invitations;

    public Connect4InvitationManager()
    {
        _invitations = new Dictionary<string, Invitation>();
    }

    public bool Add(Invitation invitation)
    {
        if (Exists(invitation.invitationId))
            return false;

        _invitations[invitation.invitationId] = invitation;
        return true;
    }

    public bool Remove(Invitation invitation)
    {
        var id = invitation.invitationId;
        if (!Exists(id))
            return false;

        return _invitations.Remove(id);
    }

    public Invitation? Get(string id)
    {
        if (Exists(id))
            return _invitations[id];

        return null;
    }

    private bool Exists(string id)
    {
        return _invitations.ContainsKey(id);
    }

    public Invitation? Create(string sender, string receiver)
    {
        var invitation = new Invitation(sender, receiver);
        return Add(invitation)
        ? invitation
        : null;

    }
}