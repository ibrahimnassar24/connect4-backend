using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using connect4_backend.Data;
using connect4_backend.Data.Models;
using Microsoft.AspNetCore.Authorization;
using connect4_backend.Hubs;
using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;
using System.Text.Json;

namespace connect4_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly Connect4Context _context;
        private readonly IHubContext<Connect4Hub> _hub;

        public NotificationController(
            Connect4Context context,
            IHubContext<Connect4Hub> hub)
        {
            _context = context;
            _hub = hub;
        }
        [HttpGet("test")]
[Authorize]
        public async Task<IActionResult> Test()
        {
            var notification = new Notification()
            {
                Id = 1,
                Receiver = User.FindFirstValue(ClaimTypes.Email),
                Message = "welcome to connect4 game",
                Link = "don't concern yourself",
                CreatedAt = new DateTime()
            };
            var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var json = JsonSerializer.Serialize(notification);
            await _hub.Clients.User(id).SendAsync("listening", json);
            return Ok();
}
        // GET: api/Notification
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Notification>>> GetNotifications()
        {
            return await _context.Notifications.ToListAsync();
        }

        // GET: api/Notification/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Notification>> GetNotification(int id)
        {
            var notification = await _context.Notifications.FindAsync(id);

            if (notification == null)
            {
                return NotFound();
            }

            return notification;
        }

        // PUT: api/Notification/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutNotification(int id, Notification notification)
        {
            if (id != notification.Id)
            {
                return BadRequest();
            }

            _context.Entry(notification).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!NotificationExists(id))
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

        // POST: api/Notification
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Notification>> PostNotification(Notification notification)
        {
            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetNotification", new { id = notification.Id }, notification);
        }

        // DELETE: api/Notification/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNotification(int id)
        {
            var notification = await _context.Notifications.FindAsync(id);
            if (notification == null)
            {
                return NotFound();
            }

            _context.Notifications.Remove(notification);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool NotificationExists(int id)
        {
            return _context.Notifications.Any(e => e.Id == id);
        }
    }
}
