using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


namespace connect4_backend.Auth;

public class AuthDbContext : IdentityDbContext<IdentityUser>
{

    public AuthDbContext(DbContextOptions<AuthDbContext> options) :
    base(options)
    { }

}