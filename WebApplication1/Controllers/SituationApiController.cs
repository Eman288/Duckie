using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SituationApiController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SituationApiController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetSituations()
        {
            var situations = await _context.Situations
                .Select(s => new
                {
                    s.Id,
                    s.Name
                })
                .ToListAsync();

            return Ok(situations);
        }

        [HttpGet("getsituationvideo/{id}")]
        public async Task<IActionResult> GetSituationVideo(int id)
        {
            var situation = await _context.Situations
                .Where(s => s.Id == id)
                .Select(s => new
                {
                    s.Id,
                    s.Name,
                    s.VideoUrl,
                    s.Pic
                })
                .FirstOrDefaultAsync();

            if (situation == null)
                return NotFound(new { error = "Situation not found" });

            return Ok(situation);
        }


        [HttpPost("watched")]
        public async Task<IActionResult> MarkAsWatched([FromBody] WatchedRequest request)
        {
            if (request == null || request.StudentId == 0 || request.SituationId == 0)
                return BadRequest(new { error = "Invalid request body" });

            var studentSituation = await _context.StudentSituations
                .FirstOrDefaultAsync(ss => ss.StudentId == request.StudentId && ss.SituationId == request.SituationId);

            if (studentSituation == null)
            {
                studentSituation = new StudentSituation
                {
                    StudentId = request.StudentId,
                    SituationId = request.SituationId,
                    IsWatched = true
                };
                _context.StudentSituations.Add(studentSituation);
            }
            else
            {
                studentSituation.IsWatched = true;
                _context.StudentSituations.Update(studentSituation);
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = "Marked as watched" });
        }

        public class WatchedRequest
        {
            public int StudentId { get; set; }
            public int SituationId { get; set; }
        }

    }
}
