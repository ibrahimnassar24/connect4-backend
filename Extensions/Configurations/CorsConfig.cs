using Microsoft.AspNetCore.Cors;

namespace connect4_backend.Extensions.Configurations
{
    public static class CorsConfig
    {
        public static IServiceCollection AddCorsConfig(this IServiceCollection services, IConfiguration configuration)
        {

            var uiUrl = configuration.GetSection("uiUrl")["default"];
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins",
                    builder => builder.WithOrigins("http://localhost:4200", "https://localhost:4200", uiUrl)
                                      .AllowAnyMethod()
                                      .AllowCredentials()
                                      .AllowAnyHeader());
            });



            return services;
        }
    }
}