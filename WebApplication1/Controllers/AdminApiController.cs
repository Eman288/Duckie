using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BCrypt.Net;
using WebApplication1.Models;
using Azure;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    public class AdminApiController : Controller
    {

        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AdminApiController(AppDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromForm] AdminLogin model)
        {
            //try
            //{
                if (!ModelState.IsValid)
                    return StatusCode(500, new { message = "An error occurred in Login: ", error = "Invalid email or password" });
            
                try {
                var admin = await _context.Admins.FirstOrDefaultAsync(a => a.Email == model.Email);
                if (admin == null || model.Password != admin.Password)
                {
                    return StatusCode(500, new { message = "An error occurred in Login: ", error = "Invalid email or password" });
                }

                HttpContext.Session.SetString("UserEmail", admin.Email);
                HttpContext.Session.SetString("type", "Admin");
                HttpContext.Session.SetString("Id", admin.Id.ToString());

                return Ok(new { message = "Login Successful!", adminData = admin });

            }
            catch (Exception ex)
            {
                // Log full exception details
                Console.WriteLine("Login database exception:");
                Console.WriteLine(ex.ToString());

                return StatusCode(500, new { message = ex.ToString(), error = ex.Message });
            }



        }


        // Create a DTO to receive parameters
        public class DashboardRequest
        {
            public string Type { get; set; }
        }

        [HttpPost("lessondisplay")]
        public IActionResult LessonDisplay([FromBody] LessonDisplayRequestDto dto)
        {
            try
            {
                int id = dto.UnitId;

                var unit = _context.Units
                    .Include(u => u.Lessons)
                    .FirstOrDefault(a => a.Id == id);

                if (unit == null)
                    return BadRequest(new { error = "Invalid unit id" });

                var userEmail = HttpContext.Session.GetString("UserEmail");
                if (string.IsNullOrEmpty(userEmail))
                    return BadRequest(new { error = "No user is logged in" });

                var userType = HttpContext.Session.GetString("type");
                if (userType != "Admin")
                    return BadRequest(new { error = "You are not entitled to access this page" });

                var user = _context.Admins.FirstOrDefault(a => a.Email == userEmail);
                if (user == null)
                    return BadRequest(new { error = "User not found" });

                var lessonDtos = unit.Lessons
                    .Select(lesson => new LessonDto
                    {
                        Id = lesson.Id,
                        Name = lesson.Name,
                    }).ToList();

                return Ok(new { user = new { user.Email },
                    unitId = unit.Id,  lessons = lessonDtos });
            }
            catch (Exception x)
            {
                return StatusCode(500, new { error = $"Error in displaying the lesson: {x.Message}" });
            }
        }

        [HttpPost("dashboard")]
        public IActionResult Dashboard([FromBody] DashboardRequest request)
        {
            try
            {
                if (request == null)
                    return BadRequest(new { error = "Invalid request body" });

                if (string.IsNullOrEmpty(request.Type))
                    return BadRequest(new { error = "Type is required" });

                var userEmail = HttpContext.Session.GetString("UserEmail");
                if (string.IsNullOrEmpty(userEmail))
                    return BadRequest(new { error = "No user is logged in" });

                var admin = _context.Admins.FirstOrDefault(a => a.Email == userEmail);
                if (admin == null)
                    return BadRequest(new { error = "Student not found" });

                var user = new { name = admin.Name };

                if (request.Type == "conversations")
                {
                    var conversations = _context.Conversations
                        .Select(c => new
                        {
                            c.Id,
                        })
                        .ToList();

                    return Ok(new { user, conversations });
                }
                else if (request.Type == "situations")
                {
                    var situations = _context.Situations
                        .Select(s => new
                        {
                            s.Id,
                            s.Name,
                            s.VideoUrl
                        })
                        .ToList();

                    return Ok(new { user, situations });
                }
                else // default: units
                {
                    var adminId = HttpContext.Session.GetString("Id");
                    var units = _context.Units
                        .Where(u => u.AdminId.ToString() == adminId)
                        .Select(u => new
                        {
                            u.Id,
                            u.Name,
                            u.level
                        })
                        .ToList();

                    return Ok(new { user, units });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
                return StatusCode(500, new { error = "An unexpected error occurred", details = ex.Message });
            }
        }

        // Logout Method
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            Response.Cookies.Delete(".AspNetCore.Session");
            return Ok(new { message = "Admin Logged Out!" });
        }

        [HttpPost("profile")] // not tested
        public async Task<IActionResult> ShowProfile()
        {
            var userEmail = HttpContext.Session.GetString("UserEmail");
            if (string.IsNullOrEmpty(userEmail))
                return StatusCode(500, new { message = "An Error Occurred in ShowProfile: ", error = "No User is logged in" });

            var admin = await _context.Admins.FirstOrDefaultAsync(s => s.Email == userEmail);

            return Ok(new { message = "Profile Lodded!", user = admin });
        }

    }
}
