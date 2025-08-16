
namespace connect4_backend.Extensions.Configurations;

public static class CookieConfig
{

    public static IServiceCollection AddCookieConfig(this IServiceCollection services)
    {
        services.ConfigureApplicationCookie(options =>
        {
            options.Cookie.Name = "connect4";
            options.Cookie.HttpOnly = true;
            options.Cookie.SameSite = SameSiteMode.None;
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            options.SlidingExpiration = true;
        });

        return services;
    }
}