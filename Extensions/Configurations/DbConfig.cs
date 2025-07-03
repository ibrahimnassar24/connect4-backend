using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;
using connect4_backend.Data;

namespace connect4_backend.Extensions.Configurations;

    public static class DbConfig
{
    public static IServiceCollection AddDbConfig(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<Connect4Context>(options =>
            options.UseSqlServer(configuration.GetConnectionString("default")));

        return services;
    }
}
