
using connect4_backend.Data;
using Microsoft.AspNetCore.Identity;

namespace connect4_backend.Services;

public class SharedService : ISharedService
{
    private readonly IServiceProvider _serviceProvider;

    public SharedService(
        IServiceProvider serviceProvider
    )
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<string?> GetUserId(string email)
    {
        using var scope = _serviceProvider.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

        var user = await userManager.FindByEmailAsync(email);

        return (user is not null)
        ? user.Id
        : null;
    }

    public async Task MarkMatchAsFinished(string id, string winner)
    {
        using var scope = _serviceProvider.CreateScope();
        var ctx = scope.ServiceProvider.GetRequiredService<Connect4Context>();

        var match = await ctx.Matches.FindAsync(id);

        if (match is null)
            return;

        match.Winner = winner;
        match.Status = "FINISHED";
        match.UpdatedAt = DateTime.UtcNow;
        await ctx.SaveChangesAsync();
    }
}