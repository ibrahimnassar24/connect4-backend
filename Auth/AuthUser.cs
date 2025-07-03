using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace connect4_backend.Auth;

public class AuthUser : IdentityUser
{

    public string? Online { get; set; }
}