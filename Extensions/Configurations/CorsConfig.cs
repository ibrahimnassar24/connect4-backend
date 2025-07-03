using Microsoft.AspNetCore.Cors;

namespace connect4_backend.Extensions.Configurations
{
    public static class CorsConfig
    {
        public static IServiceCollection AddCorsConfig(this IServiceCollection services)
        {

            services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins",
                    builder => builder.WithOrigins("http://localhost:4200", "https://localhost:4200", "http://192.168.1.6:4200")
                                      .AllowAnyMethod()
                                      .AllowCredentials()
                                      .AllowAnyHeader());
            });

            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.SameSite = SameSiteMode.Lax; // Set SameSite to Lax for cookies
            });

            return services;
        }
    }
}