using Microsoft.AspNetCore.Identity;

namespace connect4_backend.Services;

public interface IJwt
{
    Task<string> GenerateToken(IdentityUser user);
}