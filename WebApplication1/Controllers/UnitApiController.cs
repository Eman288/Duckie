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
using System.Xml;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    public class UnitApiController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public UnitApiController(AppDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpPost("index")]
        public async Task<IActionResult> Index()
        {
            var userEmail = HttpContext.Session.GetString("UserEmail");
            if (string.IsNullOrEmpty(userEmail))
                return BadRequest(new { message = "No user is logged!" });

            var student = await _context.Students.FirstOrDefaultAsync(s => s.Email == userEmail);
            if (student == null)
                return BadRequest(new { message = "No user is logged!" });

            var viewModel = new StudentProfileViewModel
            {
                Name = student.Name,
                ProfilePicture = student.Picture,
                TotalPoints = student.TotalPoints,
                JoinDate = student.JoinDate.ToShortDateString(),
                Units = _context.Units.Select(u => new UnitViewModel
                {
                    Id = u.Id,
                    Name = u.Name
                }).ToList()
            };

            return Ok(new { message = "Units are returned", user = viewModel });
        }

        [HttpPost("unitDetails")]
        public async Task<IActionResult> UnitDetails(int unitId)
        {
            var unit = _context.Units
                .Where(u => u.Id == unitId)
                .Select(u => new UnitViewModel
                {
                    Id = u.Id,
                    Name = u.Name
                }).FirstOrDefaultAsync();

            if (unit == null)
            {
                return BadRequest(new { message = "no units for this user" });
            }

            return Ok(new { message = "unit details", unitData = unit });
        }

        // Admin

        // GET: api/Units
        [HttpGet("GetUnits")]
        public async Task<ActionResult<IEnumerable<Unit>>> GetUnits()
        {
            return await _context.Units
                .Include(u => u.Admin)
                .Include(u => u.Lessons)
                .ToListAsync();
        }

        // GET: api/Units/5
        [HttpGet("GetUnit")]
        public async Task<ActionResult<Unit>> GetUnit(int id)
        {
            var unit = await _context.Units
                .Include(u => u.Admin)
                .Include(u => u.Lessons)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (unit == null)
                return NotFound();

            return unit;
        }

        // POST: api/create
        [HttpPost("CreateUnit")]
        public async Task<ActionResult<Unit>> CreateUnit([FromForm] CreateUnitDto dto)
        {
            try
            {
                var u = await _context.Units.Where(a => a.Name == dto.Name).FirstOrDefaultAsync();
                if (u != null)
                    return BadRequest(new { message = "There is a unit with this name already" });

                if (dto.QuizFile == null)
                    return BadRequest(new { message = "A Unit Must Have A Json File For The Quiz Data" });

                var adminIdStr = HttpContext.Session.GetString("Id");
                if (string.IsNullOrEmpty(adminIdStr))
                    return BadRequest(new { message = "User session missing or expired." });

                if (Path.GetExtension(dto.QuizFile.FileName).ToLower() != ".json")
                {
                    return BadRequest(new { message = "Only .json files are allowed." });
                }

                var uOrder = _context.Units.Where(a => a.OrderWithInLevel == dto.OrderWithInLevel && a.level == dto.Level).FirstOrDefault();
                if (uOrder != null)
                    return BadRequest(new { message = "a Unit already in this order" });


                // Save file
                var wwwRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                var uploadsFolder = Path.Combine(wwwRootPath, "Json", "Unit", dto.Level);
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = dto.Name + ".json";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.QuizFile.CopyToAsync(stream);
                }

                // Store the relative path/url in DB to be accessed by frontend later
                var relativeFilePath = $"/Json/Unit/{dto.Level}/{uniqueFileName}";


                // Save unit image if provided
                string imageRelativePath = null;
                if (dto.UnitImageFile != null && dto.UnitImageFile.Length > 0)
                {
                    var imagesFolder = Path.Combine(wwwRootPath, "Images", "Unit");
                    if (!Directory.Exists(imagesFolder))
                        Directory.CreateDirectory(imagesFolder);

                    var imageFileExtension = Path.GetExtension(dto.UnitImageFile.FileName).ToLower();
                    if (imageFileExtension != ".jpg" && imageFileExtension != ".jpeg" && imageFileExtension != ".png")
                    {
                        return BadRequest(new { message = "Only image files (.jpg, .jpeg, .png) are allowed." });
                    }

                    var imageFileName = dto.Name + imageFileExtension;
                    var imageFilePath = Path.Combine(imagesFolder, imageFileName);

                    using (var imageStream = new FileStream(imageFilePath, FileMode.Create))
                    {
                        await dto.UnitImageFile.CopyToAsync(imageStream);
                    }

                    imageRelativePath = $"/Images/Unit/{imageFileName}"; // store relative path for DB
                }


                var unit = new Unit
                {
                    Name = dto.Name,
                    level = dto.Level,
                    Quiz = relativeFilePath,
                    Picture = imageRelativePath,
                    AdminId = int.Parse(adminIdStr),
                    Description = dto.Description, 
                    OrderWithInLevel = dto.OrderWithInLevel,
                };

                _context.Units.Add(unit);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Creation is done" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Server error: {ex.Message}" });
            }
        }


        // PUT: api/Units/5
        [HttpPut("UpdateUnit")]
        public async Task<IActionResult> UpdateUnit(int id, [FromBody] CreateUnitDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingUnit = await _context.Units.FindAsync(id);
            if (existingUnit == null)
                return NotFound();

            existingUnit.Name = dto.Name;
            existingUnit.AdminId = dto.AdminId;
            existingUnit.level = dto.Level;
            existingUnit.Quiz = dto.Quiz; // fix this
            existingUnit.AdminId = int.Parse(HttpContext.Session.GetString("Id"));

            await _context.SaveChangesAsync();
            return NoContent();
        }


        // DELETE: api/Units/5
        [HttpDelete("DeleteUnit")]
        public async Task<IActionResult> DeleteUnit(int id)
        {
            // Find the unit
            var unit = await _context.Units.FindAsync(id);
            if (unit == null)
                return NotFound(new { message = "Unit not found" });

            // Check if the unit has lessons
            if (unit.Lessons.Count > 0)
                return BadRequest(new { message = "Cannot delete this unit because it has lessons." });

            // Check if any students are linked
            var hasStudents = await _context.StudentUnits.AnyAsync(su => su.UnitId == id);
            if (hasStudents)
                return BadRequest(new { message = "Cannot delete this unit because students have finished it." });

            //  Delete associated files
            var wwwRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

            // 1️⃣ Delete the Unit Quiz JSON file
            if (!string.IsNullOrEmpty(unit.Quiz))
            {
                var quizFilePath = Path.Combine(wwwRootPath, unit.Quiz.TrimStart('/')
                    .Replace('/', Path.DirectorySeparatorChar));
                if (System.IO.File.Exists(quizFilePath))
                {
                    System.IO.File.Delete(quizFilePath);
                }
            }

            //  Delete the Unit Picture
            if (!string.IsNullOrEmpty(unit.Picture))
            {
                var pictureFilePath = Path.Combine(wwwRootPath, unit.Picture.TrimStart('/')
                    .Replace('/', Path.DirectorySeparatorChar));
                if (System.IO.File.Exists(pictureFilePath))
                {
                    System.IO.File.Delete(pictureFilePath);
                }
            }

            //  Remove Unit from the database
            _context.Units.Remove(unit);
            await _context.SaveChangesAsync();

            return NoContent();
        }


    }
}
