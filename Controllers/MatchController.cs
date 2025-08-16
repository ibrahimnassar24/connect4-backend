using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using connect4_backend.Data;
using connect4_backend.Data.DTOs;
using connect4_backend.Data.Models;
using connect4_backend.Hubs;
using connect4_backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using NuGet.Common;
using NuGet.Protocol.Plugins;

namespace connect4_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MatchController : ControllerBase
    {
        private readonly Connect4Context _context;
        private readonly IConnect4InvitationManager _invitationManager;
        private IConnect4GameManager _gameManager;
        private readonly IGameHub _hub;
        private readonly UserManager<IdentityUser> _userManager;

        public MatchController(
            Connect4Context context,
            IGameHub gameHub,
            UserManager<IdentityUser> userManager,
            IConnect4GameManager gameManager,
            IConnect4InvitationManager invitationManager)
        {
            _context = context;
            _hub = gameHub;
            _userManager = userManager;
            _gameManager = gameManager;
            _invitationManager = invitationManager;
        }


        // post api/match/invite/some@example.com
        [HttpPost("invite")]
        [Authorize]
        public async Task<IActionResult> CreateInvitation(InvitationRequest invitationRequest)
        {
            // create an invitation with the current user as the sender
            // and the received email as the receiver

            var sender = User.FindFirstValue(ClaimTypes.Email);
            var email = invitationRequest.email;
            var invitation = _invitationManager.Create(sender, email);
            if (invitation is null)
                return BadRequest();

            // send notification to the receiver
            await _hub.SendInvitationNotification(invitation);

            return Ok(new
            {
                invitationId = invitation.invitationId
            });
        }

        [HttpPut("accept/{id}")]
        [Authorize]
        public async Task<IActionResult> Accept(string id)
        {
            // check if the invitation exists and is not expired
            var invitation = _invitationManager.Get(id);
            if (invitation is null)
                return NotFound();

            if (invitation.HasExpired())
            {
                _invitationManager.Remove(invitation);
                return BadRequest("invitation has expired");
            }

            // create the match and save it to database
            var match = new Match();
            match.Id = Guid.NewGuid().ToString();
            match.FirstPlayer = invitation.senderEmail;
            match.SecondPlayer = invitation.receiverEmail;
            match.CreatedAt = DateTime.UtcNow;
            match.UpdatedAt = DateTime.UtcNow;
            match.Status = "ONGOING";

            _context.Matches.Add(match);
            await _context.SaveChangesAsync();

            // create the transfer data object
            var dto = new MatchDto(match);
            dto.invitationId = id;

            // create gamesession for the match.
            var gameSession = _gameManager.CreateGameSession(dto);

            // notify the invitation sender about match details.
            await _hub.SendInvitationAcceptanceNotification(match.FirstPlayer, invitation.invitationId, match.Id);

            // return the match details to the invitation receiver
            return Ok(new { matchId = dto.id});
        }


        [HttpDelete("decline/{id}")]
        [Authorize]
        public async Task<IActionResult> Decline(string id)
        {
            var invitation = _invitationManager.Get(id);
            if (invitation is null)
                return NoContent();

            await _hub.DeclineInvitation(invitation);
            _invitationManager.Remove(invitation);
            return NoContent();
        }

        [HttpDelete("withdraw/{id}")]
        [Authorize]
        public IActionResult Withdraw(string id)
        {
            var invitation = _invitationManager.Get(id);
            if (invitation is null)
                return NotFound();

            var removed = _invitationManager.Remove(invitation);
            if (removed)
                return NoContent();

            return BadRequest();
        }
    }
}