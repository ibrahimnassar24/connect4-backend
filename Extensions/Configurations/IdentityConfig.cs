using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;
using connect4_backend.Auth;

namespace connect4_backend.Extensions.Configurations
{
    public static class IdentityConfig
    {
        public static IServiceCollection AddIdentityConfig(
            this IServiceCollection services,
            IConfiguration configuration)
        {

            services.AddDbContext<AuthDbContext>(
    options => options.UseSqlServer(configuration.GetConnectionString("auth"))
);

services.AddIdentityApiEndpoints<IdentityUser>()
    .AddEntityFrameworkStores<AuthDbContext>()
        .AddDefaultTokenProviders();


            return services;
        }
    }
}