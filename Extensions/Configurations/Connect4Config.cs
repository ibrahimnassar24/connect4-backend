using connect4_backend.Services;

namespace connect4_backend.Extensions.Configurations;

public static class Connect4Config
{
    public static IServiceCollection AddConnect4Config(this IServiceCollection services, IConfiguration configurations)
    {
        services.AddSingleton<IConnect4GameManager, Connect4GameManager>();
        services.AddSingleton<IConnect4InvitationManager, Connect4InvitationManager>();
        services.AddSingleton<ISharedService, SharedService>();
        services.AddSingleton<IGameHub, GameHub>();
        
        return services;
    }
}