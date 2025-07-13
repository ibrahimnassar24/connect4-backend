using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;
using Microsoft.AspNetCore.Cors;
using connect4_backend.Auth;
using connect4_backend.Extensions.Configurations;
using connect4_backend.Hubs;
using connect4_backend.Services;


var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(ops =>
{
    ops.ListenAnyIP(5286);
});
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Add services to the container.
builder.Services.AddSingleton<IConnect4GameManager, Connect4GameManager>();
builder.Services.AddCorsConfig(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddAuthorization();
builder.Services.AddIdentityConfig(builder.Configuration);
builder.Services.AddDbConfig(builder.Configuration);
builder.Services.AddSignalR();




// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAllOrigins");
app.MapIdentityApi<IdentityUser>();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapHub<Connect4Hub>("/hub")
.RequireAuthorization();

app.MapPost("/logout", async (SignInManager<IdentityUser> signInManager,
    [FromBody] object empty) =>
{
    if (empty != null)
    {
        await signInManager.SignOutAsync();
        return Results.Ok();
    }
    return Results.Unauthorized();
})
// .WithOpenApi()
.RequireAuthorization();

app.Run();
