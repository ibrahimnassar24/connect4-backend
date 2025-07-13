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
using Microsoft.AspNetCore.Authorization;
using NuGet.Common;
using Microsoft.AspNetCore.SignalR;
using connect4_backend.Hubs;
using Microsoft.AspNetCore.Identity;
using NuGet.Protocol.Plugins;
using System.Security.Claims;
using connect4_backend.Services;

namespace connect4_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MatchController : ControllerBase
    {
        private readonly Connect4Context _context;
        private readonly IHubContext<Connect4Hub> _hub;
        private IConnect4GameManager _gameManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<MatchController> _logger;

        public MatchController(
            Connect4Context context,
            IHubContext<Connect4Hub> hub,
            UserManager<IdentityUser> userManager,
            ILogger<MatchController> logger,
            IConnect4GameManager gameManager)
        {
            _context = context;
            _hub = hub;
            _userManager = userManager;
            _gameManager = gameManager;
            _logger = logger;
        }

        // GET: api/Match
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Match>>> GetMatches()
        {
            return await _context.Matches.ToListAsync();
        }

        // GET: api/Match/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Match>> GetMatch(int id)
        {
            var match = await _context.Matches.FindAsync(id);

            if (match == null)
            {
                return NotFound();
            }

            return match;
        }

        // PUT: api/Match/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMatch(int id, Match match)
        {
            if (id != match.Id)
            {
                return BadRequest();
            }

            _context.Entry(match).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MatchExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Match
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Match>> PostMatch(MatchRequest matchRequest)
        {
            var match = new Match()
            {
                FirstPlayer = matchRequest.firstPlayer,
                SecondPlayer = matchRequest.secondPlayer,
                Status = "PENDING",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            _context.Matches.Add(match);
            await _context.SaveChangesAsync();
            var secondPlayer = await _userManager.FindByEmailAsync(match.SecondPlayer);
            var notification = Notification.CreateGameInvitation(match.FirstPlayer);
            notification.Receiver = match.SecondPlayer;
            notification.Link = $"{match.Id}";
            var data = notification.ToJson();

            await _hub.Clients.User(secondPlayer.Id).SendAsync("notification", data);

            return CreatedAtAction("GetMatch", new { id = match.Id }, match);
        }

        [HttpGet("accept/{id}")]
        public async Task<ActionResult<MatchDto>> AcceptMatch(int id)
        {
            var match = await _context.Matches.FindAsync(id);
            match.Status = "ONGOING";
            await _context.SaveChangesAsync();

            var matchDto = new MatchDto(match);
            matchDto.turn = matchDto.firstPlayer;
            var data = matchDto.ToJson();
            var firstPlayer = await _userManager.FindByEmailAsync(match.FirstPlayer);
            var firstPlayerId = firstPlayer.Id;
            var secondPlayerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _hub.Clients.User(firstPlayerId).SendAsync("match", data);
            var session = new GameSession()
            {
                GameId = match.Id,
                FirstPlayerEmail = match.FirstPlayer,
                FirstPlayerId = firstPlayerId,
                SecondPlayerEmail = match.SecondPlayer,
                SecondPlayerId = secondPlayerId,
                Turn = match.FirstPlayer
            };
            _gameManager.AddGameSession(session);
            return Ok(matchDto);
        }


        // DELETE: api/Match/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMatch(int id)
        {
            var match = await _context.Matches.FindAsync(id);
            if (match == null)
            {
                return NotFound();
            }

            _context.Matches.Remove(match);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MatchExists(int id)
        {
            return _context.Matches.Any(e => e.Id == id);
        }
    }
}
