using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace connect4_backend.Controllers;

[Route("api/auth-google")]
[ApiController]
public class GoogleAuthController : ControllerBase
{

    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly UserManager<IdentityUser> _userManager;

    public GoogleAuthController(
        SignInManager<IdentityUser> signInManager,
        UserManager<IdentityUser> userManager
    )
    {
        _signInManager = signInManager;
        _userManager = userManager;
    }

    [HttpGet("login")]
    public IActionResult GoogleLogin()
    {
        var properties = _signInManager.ConfigureExternalAuthenticationProperties(GoogleDefaults.AuthenticationScheme, "https://localhost:4200");
        return Challenge(properties, GoogleDefaults.AuthenticationScheme);
    }


    [HttpGet("callback")]
    public async Task<IActionResult> GoogleCallback()
    {
        Console.WriteLine("from callback");
        var returnUrl = "http://localhost:4200";


        var info = await _signInManager.GetExternalLoginInfoAsync();
        if (info is null)
            return BadRequest();

        // Try sign in (if user already linked)
        var signInResult = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);
        if (signInResult.Succeeded)
        {
            // successful: redirect back to frontend
            return LocalRedirect(returnUrl);
        }

        // If user doesn't exist, create and link
        var email = info.Principal.FindFirstValue(ClaimTypes.Email);
        if (email == null) return Redirect(returnUrl);

        var user = new IdentityUser { UserName = email, Email = email };
        var createResult = await _userManager.CreateAsync(user);
        if (!createResult.Succeeded) return Redirect(returnUrl);

        var addLogin = await _userManager.AddLoginAsync(user, info);
        if (!addLogin.Succeeded) return Redirect(returnUrl);

        await _signInManager.SignInAsync(user, isPersistent: false);
        Console.WriteLine("before the end");
        return LocalRedirect(returnUrl);

    }
}