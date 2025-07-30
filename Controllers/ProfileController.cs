using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using connect4_backend.Data;
using connect4_backend.Data.Models;
using connect4_backend.Data.DTOs;
using Microsoft.AspNetCore.Identity;
using connect4_backend.Auth;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;
using NuGet.Common;

namespace connect4_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly Connect4Context _context;

        public ProfileController(
            Connect4Context context)
        {
            _context = context;
        }


        // GET: api/Profile/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProfile(string id)
        {
            var profile = await _context.Profiles.FindAsync(id);

            if (profile is null)
                return NotFound();

            return Ok(ProfileDto.fromProfile(profile));
        }

        // Patch: api/Profile/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize]
        [HttpPatch]
        public async Task<IActionResult> PatchProfile(ProfileFormData data)
        {
            // get the email for the signed in user.
            var email = User.FindFirstValue(ClaimTypes.Email);

            // get profile if exists, if not returns null.
            var profile = await _context.Profiles.FindAsync(email);

            if (profile is null)
                return NotFound();

            profile.FirstName = (string.IsNullOrEmpty(data.firstName))
            ? profile.FirstName
            : data.firstName;

            profile.LastName = (string.IsNullOrEmpty(data.lastName))
            ? profile.LastName
            : data.lastName;

            profile.Bio = (string.IsNullOrEmpty(data.bio))
            ? profile.Bio
            : data.bio;

            profile.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(ProfileDto.fromProfile(profile));
        }

        // POST: api/Profile
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Profile>> PostProfile(ProfileFormData data)
        {
            // get the email for the signed in user.
            var email = User.FindFirstValue(ClaimTypes.Email);

            var profile = new Profile()
            {
                Email = email,
                FirstName = data.firstName,
                LastName = data.lastName,
                Bio = data.bio,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                AvatarUrl = "/user-avatar.png",
                CoverUrl = "/cover.jpg"
            };

            _context.Profiles.Add(profile);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (ProfileExists(profile.Email))
                    return Conflict();
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetProfile", new { id = profile.Email }, ProfileDto.fromProfile(profile));
        }

        // DELETE: api/Profile/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProfile(string id)
        {
            var profile = await _context.Profiles.FindAsync(id);
            if (profile == null)
            {
                return NotFound();
            }

            _context.Profiles.Remove(profile);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search(string query)
        {
            if (String.IsNullOrEmpty(query))
                return Ok(new List<ProfileSearchResult>());

            var profiles = await _context.Profiles.ToListAsync();

            var results = profiles.Where(p =>
            p.Email.Contains(query) ||
            p.FirstName.Contains(query) ||
            p.LastName.Contains(query))
            .Select(p => new ProfileSearchResult()
            {
                email = p.Email,
                firstName = p.FirstName,
                lastName = p.LastName,
                avatarUrl = p.AvatarUrl
            }).ToList();

            return Ok(results);
        }

        private bool ProfileExists(string id)
        {
            return _context.Profiles.Any(e => e.Email == id);
        }
    }
}
