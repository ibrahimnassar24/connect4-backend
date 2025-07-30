using connect4_backend.Data.DTOs;
using System.Collections.Generic;

namespace connect4_backend.Services;

public interface IConnect4InvitationManager
{

    public bool Add(Invitation invitation);
    public bool Remove(Invitation invitation);
    public Invitation? Get(string id);
    public Invitation? Create(string sender, string receiver);
    
}