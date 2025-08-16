using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc;

namespace connect4_backend.Extensions.Configurations;

public static class GoogleAuthConfig
{
    public static IServiceCollection AddGoogleAuthConfig(this IServiceCollection services, IConfiguration configurations)
    {
        services.AddAuthentication(options =>
        {
            options.DefaultScheme = IdentityConstants.ApplicationScheme;
            options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
        })
        .AddCookie(IdentityConstants.ExternalScheme)
                .AddGoogle(options =>
        {
            options.ClientId = configurations["googleAuth:clientId"]!;
            options.ClientSecret = configurations["googleAuth:clientSecret"]!;
            options.CallbackPath = "/api/auth-google/callback";
            options.SignInScheme = IdentityConstants.ExternalScheme;
            // options.CorrelationCookie.SameSite = SameSiteMode.Unspecified;
            // options.CorrelationCookie.SecurePolicy = CookieSecurePolicy.Always;
            // options.CorrelationCookie.HttpOnly = true;
        });
        



        return services;
    }
}