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
    public class LessonApiController : Controller
    {

        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public LessonApiController(AppDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        //public IActionResult Index()
        //{
        //    return View();
        //}

        //[HttpGet]
        //public IActionResult QuizStartMenu()
        //{
        //    return View();
        //}
        [HttpGet("lessons")]
        public IActionResult Lessons(int unitId)
        {
            var unit = _context.Units.FirstOrDefault(u => u.Id == unitId);
            if (unit == null)
            {
                return BadRequest(new { message = "There is no unit with this id" });
            }

            var userEmail = HttpContext.Session.GetString("UserEmail");
            if (string.IsNullOrEmpty(userEmail))
                return BadRequest(new { error = "No user is logged in" });

            var student = _context.Students.FirstOrDefault(s => s.Email == userEmail);
            if (student == null)
                return BadRequest(new { error = "Student not found" });

            // Get StudentLessons for the current student as a dictionary for fast lookup
            var studentLessonStatuses = _context.StudentLessons
                .Where(sl => sl.StudentId == student.Id)
                .ToDictionary(sl => sl.LessonId, sl => sl.IsDone);

            // Get lessons for the unit, and include isDone from studentLessonStatuses if exists, else false
            var lessons = _context.Lessons
                .Where(l => l.UnitId == unitId)
                .Select(l => new LessonViewModel
                {
                    Id = l.Id,
                    Name = l.Name,
                    Content = l.content,
                    Quiz = l.quizContent,
                    isDone = studentLessonStatuses.ContainsKey(l.Id) ? studentLessonStatuses[l.Id] : false
                }).ToList();

            return Ok(new { lessons = lessons, unit = unit });
        }


        [HttpPost("lessonContent")]
        public IActionResult LessonContent(int lessonId)
        {
            var lesson = _context.Lessons
                .Where(l => l.Id == lessonId)
                .Select(l => new LessonContentViewModel
                {
                    Title = l.Name,
                    Content = l.content,
                    UnitId = l.UnitId
                })
                .FirstOrDefault();

            if (lesson == null)
            {
                return BadRequest(new { message = "no lessons to show data" });
            }

            return Ok(new { message = "lesson content", lessonData = lesson, unitID = lesson.UnitId });
        }

        [HttpGet("saveLesson")]
        public IActionResult SaveLesson(int lessonId)
        {
            var email = HttpContext.Session.GetString("UserEmail");
            if (email == null)
            {
                return BadRequest(new { error = "There is no logged in user" });
            }

            // STEP 1: Find the user and include related data
            var user = _context.Students
                .Include(s => s.StudentLessons)
                .Include(s => s.StudentUnits)
                .FirstOrDefault(a => a.Email == email);

            if (user == null)
            {
                return StatusCode(500, new { error = "User not found for email: " + email });
            }

            // DEBUG INFO
            var lessonList = _context.StudentLessons
                .Where(sl => sl.StudentId == user.Id)
                .ToList();

            if (!lessonList.Any())
            {
                return StatusCode(500, new
                {
                    error = "No StudentLessons found for user.",
                    userId = user.Id
                });
            }

            // STEP 2: Find the specific StudentLesson
            var sL = lessonList.FirstOrDefault(a => a.LessonId == lessonId);
            if (sL == null)
            {
                return StatusCode(500, new
                {
                    error = "StudentLesson not found",
                    lessonId = lessonId,
                    availableLessonIds = lessonList.Select(sl => sl.LessonId).ToList()
                });
            }

            try
            {
                // STEP 3: Mark lesson as done and add points
                sL.IsDone = true;
                user.DailyPoints += 20;
                user.TotalPoints += 20;

                // STEP 4: Load all lessons
                var allLessons = _context.Lessons
                    .Include(l => l.Unit) // IMPORTANT for level and OrderWithInLevel
                    .OrderBy(l => l.Unit.level)
                    .ThenBy(l => l.Unit.OrderWithInLevel)
                    .ThenBy(l => l.OrderWithInLevel)
                    .ToList();

                var currentLesson = allLessons.FirstOrDefault(l => l.Id == lessonId);
                if (currentLesson == null)
                {
                    return StatusCode(500, new { error = "Lesson entity not found in database.", lessonId = lessonId });
                }

                var currentUnitId = currentLesson.UnitId;

                // STEP 5: Get lessons in the current unit
                var lessonsInUnit = allLessons
                    .Where(l => l.UnitId == currentUnitId)
                    .OrderBy(l => l.OrderWithInLevel)
                    .ToList();

                int lessonIndex = lessonsInUnit.FindIndex(l => l.Id == lessonId);

                if (lessonIndex >= 0 && lessonIndex < lessonsInUnit.Count - 1)
                {
                    // STEP 6: Mark next lesson as done
                    var nextLessonId = lessonsInUnit[lessonIndex + 1].Id;

                    var nextStudentLesson = user.StudentLessons
                        .FirstOrDefault(sl => sl.LessonId == nextLessonId);

                    if (nextStudentLesson != null && !nextStudentLesson.IsDone)
                    {
                        nextStudentLesson.IsDone = true;
                    }
                }
                else
                {
                    // STEP 7: All lessons in the unit are done -> Mark Unit as Done
                    var studentUnit = user.StudentUnits.FirstOrDefault(su => su.UnitId == currentUnitId);
                    if (studentUnit != null)
                    {
                        studentUnit.IsDone = true;

                        var allUnits = _context.Units
                            .OrderBy(u => u.level)
                            .ThenBy(u => u.OrderWithInLevel)
                            .ToList();

                        int unitIndex = allUnits.FindIndex(u => u.Id == currentUnitId);

                        if (unitIndex >= 0 && unitIndex < allUnits.Count - 1)
                        {
                            var nextUnit = allUnits[unitIndex + 1];
                            var nextStudentUnit = user.StudentUnits.FirstOrDefault(su => su.UnitId == nextUnit.Id);

                            if (nextStudentUnit != null && !nextStudentUnit.IsDone)
                            {
                                nextStudentUnit.IsDone = true;

                                var firstLessonNextUnit = _context.Lessons
                                    .Where(l => l.UnitId == nextUnit.Id)
                                    .OrderBy(l => l.OrderWithInLevel)
                                    .FirstOrDefault();

                                if (firstLessonNextUnit != null)
                                {
                                    var nextStudentLesson2 = user.StudentLessons
                                        .FirstOrDefault(sl => sl.LessonId == firstLessonNextUnit.Id);

                                    if (nextStudentLesson2 != null && !nextStudentLesson2.IsDone)
                                    {
                                        nextStudentLesson2.IsDone = true;
                                    }
                                }
                            }
                        }
                    }
                }

                // STEP 8: Save
                _context.SaveChanges();
                return Ok(new { success = true, lessonId = lessonId });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = "Error saving changes",
                    lessonId = lessonId,
                    details = ex.Message,
                    stackTrace = ex.StackTrace
                });
            }
        }

        // Admin Section

        // GET: api/Lessons
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Lesson>>> GetLessons()
        {
            return await _context.Lessons
                .Include(l => l.Admin)
                .Include(l => l.Unit)
                .Include(l => l.Questions)
                .ToListAsync();
        }

        // GET: api/Lessons/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Lesson>> GetLesson(int id)
        {
            var lesson = await _context.Lessons
                .Include(l => l.Admin)
                .Include(l => l.Unit)
                .Include(l => l.Questions)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (lesson == null)
                return NotFound();

            return lesson;
        }



        // POST: api/Lessons
        [HttpPost]
        public async Task<ActionResult<Lesson>> CreateLesson([FromBody] CreateLessonDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            //  Check for duplicate name (case-insensitive)
            var existing = await _context.Lessons
                .AnyAsync(l => l.Name.ToLower() == dto.Name.ToLower());

            if (existing)
                return BadRequest(new { message = "Lesson name already exists." });

            if (dto.Content == null)
                return BadRequest(new { message = "A Lesson Must Have A Json File For The content Data" });

            if (dto.Quiz == null)
                return BadRequest(new { message = "A Lesson Must Have A Json File For The Quiz Data" });

            var adminIdStr = HttpContext.Session.GetString("Id");
            if (string.IsNullOrEmpty(adminIdStr))
                return BadRequest(new { message = "User session missing or expired." });

            if (HttpContext.Session.GetString("type") != "Admin")
                return BadRequest(new { message = "You do not have access to this page" });

            if (Path.GetExtension(dto.Quiz.FileName).ToLower() != ".json" || Path.GetExtension(dto.Content.FileName).ToLower() != ".json")
            {
                return BadRequest(new { message = "Only .json files are allowed." });
            }

            var unit = _context.Units.Where(a => a.Id == dto.UnitId).FirstOrDefault(); // get the unit id
            // Save Content File
            var wwwRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var uploadsFolder = Path.Combine(wwwRootPath, "Json", "Lesson", unit.level, $"Unit{unit.OrderWithInLevel.ToString()}");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = dto.Name + ".json";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await dto.Content.CopyToAsync(stream);
            }

            // Store the relative path/url in DB to be accessed by frontend later
            var relativeFilePath = $"/Json/Lesson/{unit.level}/Unit{unit.OrderWithInLevel.ToString()}/{uniqueFileName}";

            // Save Quiz File
            wwwRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            uploadsFolder = Path.Combine(wwwRootPath, "Json", "Quiz", unit.level, $"Unit{unit.OrderWithInLevel.ToString()}");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            uniqueFileName = dto.Name + ".json";
            filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await dto.Quiz.CopyToAsync(stream);
            }

            // Store the relative path/url in DB to be accessed by frontend later
            var relativeFilePathQ = $"/Json/Quiz/{unit.level}/Unit{unit.OrderWithInLevel.ToString()}/{uniqueFileName}";


            var lesson = new Lesson
            {
                Name = dto.Name,
                Pictures = dto.Pictures,
                UnitId = dto.UnitId,
                AdminId = dto.AdminId,
                content = relativeFilePath,
                quizContent = relativeFilePathQ
            };

            

            _context.Lessons.Add(lesson);
            unit.Lessons.Add(lesson);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Creation is done" });
        }

        // PUT: api/Lessons/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLesson(int id, [FromBody] CreateLessonDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingLesson = await _context.Lessons.FindAsync(id);
            if (existingLesson == null)
                return NotFound();

            //  Check if the new name exists for another lesson
            var duplicate = await _context.Lessons
                .AnyAsync(l => l.Id != id && l.Name.ToLower() == dto.Name.ToLower());

            if (duplicate)
                return BadRequest(new { message = "Another lesson with the same name already exists." });

            existingLesson.Name = dto.Name;
            existingLesson.Pictures = dto.Pictures;
            existingLesson.UnitId = dto.UnitId;
            existingLesson.AdminId = dto.AdminId;
            //existingLesson.content = dto.Content;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/Lessons/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLesson(int id)
        {
            // Find the unit
            var lesson = await _context.Lessons.FindAsync(id);
            if (lesson == null)
                return NotFound(new { message = "Lesson not found" });

            // Check if there are any students linked to this lesson
            var hasStudents = await _context.StudentLessons.AnyAsync(su => su.LessonId == id);
            if (hasStudents)
                return BadRequest(new { message = "Cannot delete this lesson because there are students finished it." });

            // Remove lesson
            _context.Lessons.Remove(lesson);
            await _context.SaveChangesAsync();

            return NoContent();
        }


    }
}
