using connect4_backend.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class UploadController : ControllerBase
{
    private readonly IWebHostEnvironment _env;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly Connect4Context _context;

    public UploadController(
        IWebHostEnvironment env,
        Connect4Context context,
    UserManager<IdentityUser> userManager)
    {
        _env = env;
        _context = context;
        _userManager = userManager;
    }

    [HttpPost("avatar")]
    [Authorize]

    public async Task<IActionResult> UploadAvatar(IFormFile file)
    {
        if (file is null || file.Length == 0)
            return BadRequest("No file uploaded.");

        var url = await UploadImage("avatars", file);

        var email = User.FindFirstValue(ClaimTypes.Email);
        var profile = await _context.Profiles.FindAsync(email);
        if (profile is not null)
        {
            profile.AvatarUrl = url;
            await _context.SaveChangesAsync();
        }

        return Ok(new { url });
    }

    [HttpPost("cover")]
    [Authorize]

    public async Task<IActionResult> UploadCover(IFormFile file)
    {
        if (file is null || file.Length == 0)
            return BadRequest("No file uploaded.");

        var url = await UploadImage("covers", file);

                var email = User.FindFirstValue(ClaimTypes.Email);
        var profile = await _context.Profiles.FindAsync(email);
        if (profile is not null)
        {
            profile.CoverUrl = url;
            await _context.SaveChangesAsync();
        }

        return Ok(new { url });
    }

    private async Task<string> UploadImage(string dir, IFormFile image)
    {
        var uploadsFolder = Path.Combine(_env.WebRootPath ?? Path.Combine(_env.ContentRootPath, "wwwroot"), dir);

        if (!Directory.Exists(uploadsFolder))
            Directory.CreateDirectory(uploadsFolder);

        var imageId = Guid.NewGuid().ToString();
        var fileName = $"{imageId}.png";
        var filePath = Path.Combine(uploadsFolder, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await image.CopyToAsync(stream);
        }

        var url = $"{Request.Scheme}://{Request.Host}/{dir}/{fileName}";
        return url;
    }
}
