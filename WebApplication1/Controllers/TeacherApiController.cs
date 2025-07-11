using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;
using BCrypt.Net;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TeacherApiController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TeacherApiController(AppDbContext context)
        {
            _context = context;
        }

        // POST: api/teacher/add-course
        [HttpPost("add-course")]
        public async Task<IActionResult> AddCourse([FromForm] CourseCreateDto dto)
        {
            var teacherEmail = HttpContext.Session.GetString("UserEmail");
            if (teacherEmail == null)
            {
                return Unauthorized("Teacher is not logged in.");
            }

            if (HttpContext.Session.GetString("type") != "Teacher")
            {
                return Unauthorized("A Teacher must be logged in to access this page");
            }

            var teacher = await _context.Teachers.FirstOrDefaultAsync(t => t.Email == teacherEmail);
            if (teacher == null || !teacher.IsActive)
            {
                return Unauthorized("Teacher not authorized or inactive.");
            }

            if (dto.Picture == null || dto.Picture.Length == 0)
            {
                return BadRequest("Course image is required.");
            }

            // Create folder if it doesn't exist
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", "Course");
            Directory.CreateDirectory(folderPath);

            // Generate unique image name
            var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(dto.Picture.FileName)}";
            var filePath = Path.Combine(folderPath, uniqueFileName);

            // Save the image to the folder
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await dto.Picture.CopyToAsync(stream);
            }

            var relativePath = $"/Images/Course/{uniqueFileName}";

            var course = new Course
            {
                TeacherId = teacher.Id,
                Title = dto.Title,
                Description = dto.Description,
                Price = dto.Price,
                Picture = relativePath,
                IsActive = false // the course is unactive until the teacher choose to post it
            };

            _context.Courses.Add(course);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Course added successfully", courseId = course.Id });
        }

        // PUT: api/teacher/update-course
        [HttpPut("update-course")]
        public async Task<IActionResult> UpdateCourse([FromForm] CourseUpdateDto dto)
        {
            var teacherEmail = HttpContext.Session.GetString("UserEmail");
            if (teacherEmail == null)
            {
                return Unauthorized("Teacher is not logged in.");
            }

            if (HttpContext.Session.GetString("type") != "Teacher")
            {
                return Unauthorized("A Teacher must be logged in to access this page");
            }

            var teacher = await _context.Teachers.FirstOrDefaultAsync(t => t.Email == teacherEmail);
            if (teacher == null || !teacher.IsActive)
            {
                return Unauthorized("Teacher not authorized or inactive.");
            }

            var course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == dto.Id && c.TeacherId == teacher.Id);
            if (course == null)
            {
                return NotFound("Course not found or you don't have permission to update it.");
            }

            // Update text fields if provided
            course.Title = dto.Title ?? course.Title;
            course.Description = dto.Description ?? course.Description;
            course.Price = dto.Price ?? course.Price;

            // Handle new image
            if (dto.Picture != null && dto.Picture.Length > 0)
            {
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", "Course");
                Directory.CreateDirectory(folderPath);

                // Delete old image if exists
                if (!string.IsNullOrEmpty(course.Picture))
                {
                    var oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", course.Picture.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString()));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                // Save new image
                var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(dto.Picture.FileName)}";
                var newImagePath = Path.Combine(folderPath, uniqueFileName);

                using (var stream = new FileStream(newImagePath, FileMode.Create))
                {
                    await dto.Picture.CopyToAsync(stream);
                }

                course.Picture = $"/Images/Course/{uniqueFileName}";
            }

            _context.Courses.Update(course);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Course updated successfully", courseId = course.Id });
        }


        // POST: api/teacher/add-courseContent
        [HttpPost("add-courseContent")]
        public async Task<IActionResult> AddCourseContent([FromForm] CourseContentCreateDto dto)
        {
            var teacherEmail = HttpContext.Session.GetString("UserEmail");
            if (teacherEmail == null)
            {
                return Unauthorized("Teacher is not logged in.");
            }

            if (HttpContext.Session.GetString("type") != "Teacher")
            {
                return Unauthorized("A Teacher must be logged in to access this page");
            }

            var teacher = await _context.Teachers.FirstOrDefaultAsync(t => t.Email == teacherEmail);
            if (teacher == null || !teacher.IsActive)
            {
                return Unauthorized("Teacher not authorized or inactive.");
            }

            if (dto.CourseId == null || dto.Content == null)
            {
                return BadRequest("CourseId and file are required.");
            }

            var course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == dto.CourseId);
            if (course == null)
            {
                return NotFound("Course not found.");
            }

            // Sanitize and build file path
            string courseNameSanitized = course.Title.Replace(" ", "_"); // or use Regex to remove special characters
            string fileName = $"{courseNameSanitized}.json";
            string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Json", "Course");
            Directory.CreateDirectory(folderPath); // Ensure the folder exists

            string filePath = Path.Combine(folderPath, fileName);

            // Save the file
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await dto.Content.CopyToAsync(stream);
            }

            // Save to DB
            var courseContent = new CourseContent
            {
                CourseId = dto.CourseId.Value,
                ContentUrl = $"/Json/Course/{fileName}", // relative URL to be used in frontend
                Title = dto.Title,
            };

            _context.CourseContents.Add(courseContent);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Course content uploaded successfully", courseId = course.Id });
        }

        [HttpPost("update-courseContent")]
        public async Task<IActionResult> UpdateCourseContent([FromForm] CourseContentUpdateDto dto)
        {
            var teacherEmail = HttpContext.Session.GetString("UserEmail");
            if (teacherEmail == null)
            {
                return Unauthorized("Teacher is not logged in.");
            }

            if (HttpContext.Session.GetString("type") != "Teacher")
            {
                return Unauthorized("A Teacher must be logged in to access this page");
            }

            var teacher = await _context.Teachers.FirstOrDefaultAsync(t => t.Email == teacherEmail);
            if (teacher == null || !teacher.IsActive)
            {
                return Unauthorized("Teacher not authorized or inactive.");
            }

            if (dto.Id == null || dto.Content == null)
            {
                return BadRequest("CourseContent Id and file are required.");
            }

            var courseContent = await _context.CourseContents
                .Include(c => c.Course)
                .FirstOrDefaultAsync(c => c.Id == dto.Id);

            if (courseContent == null)
            {
                return NotFound("Course content not found.");
            }

            if (courseContent.Title != dto.Title && dto.Title != "")
            {
                courseContent.Title = dto.Title;
            }

            // Build path from course name and content ID
            var courseNameSanitized = courseContent.Course.Title.Replace(" ", "_");
            var fileName = $"{courseNameSanitized}_content_{dto.Id}.json";
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Json", "Course");
            Directory.CreateDirectory(folderPath); // Ensure folder exists

            var filePath = Path.Combine(folderPath, fileName);

            // Delete the old file if it exists
            var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", courseContent.ContentUrl?.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString()));
            if (System.IO.File.Exists(oldFilePath))
            {
                System.IO.File.Delete(oldFilePath);
            }

            // Save new file
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await dto.Content.CopyToAsync(stream);
            }

            // Update DB entry
            courseContent.ContentUrl = $"/Json/Course/{fileName}";
            _context.CourseContents.Update(courseContent);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Course content updated successfully", contentId = courseContent.Id });
        }


        // GET: api/teacher/view-course/{id}
        [HttpGet("view-course/{id}")]
        public async Task<IActionResult> ViewCourse(int id)
        {
            var teacherEmail = HttpContext.Session.GetString("UserEmail");
            if (teacherEmail == null)
            {
                return Unauthorized("Teacher is not logged in.");
            }

            if (HttpContext.Session.GetString("type") != "Teacher")
            {
                return Unauthorized("A Teacher must be logged in to access this page");
            }

            var teacher = await _context.Teachers.FirstOrDefaultAsync(t => t.Email == teacherEmail);
            if (teacher == null || !teacher.IsActive)
            {
                return Unauthorized("Teacher not authorized or inactive.");
            }

            var course = await _context.Courses
                .Include(c => c.CourseContents)
                .FirstOrDefaultAsync(c => c.Id == id && c.TeacherId == teacher.Id);

            if (course == null)
            {
                return NotFound("Course not found or you don't have permission to view it.");
            }

            return Ok(new
            {
                Id = course.Id,
                Title = course.Title,
                Description = course.Description,
                Price = course.Price,
                Picture = course.Picture,
                IsActive = course.IsActive,
                Contents = course.CourseContents.Select(cc => new CourseContentDto
                {
                    Id = cc.Id,
                    Title = cc.Title
                }).ToList()
            });

        }

        [HttpGet("view-courseContent/{id}")]
        public async Task<IActionResult> ViewCourseContent(int id)
        {
            var studentEmail = HttpContext.Session.GetString("UserEmail");
            if (studentEmail == null)
            {
                return Unauthorized("Student is not logged in.");
            }

            if (HttpContext.Session.GetString("type") != "Student")
            {
                return Unauthorized("A student must be logged in to access this page");
            }

            var student= await _context.Students.FirstOrDefaultAsync(t => t.Email == studentEmail);
            if (student == null || !student.IsActive)
            {
                return Unauthorized("student not authorized or inactive.");
            }

            // make sure that the current user purchased this course
            var courseContent = _context.CourseContents.Where(a => a.Id == id).FirstOrDefault();

            var course = await _context.Courses
                .FirstOrDefaultAsync(c => c.Id == courseContent.CourseId);

            if (course == null)
            {
                return NotFound("Course not found or you don't have permission to view it.");
            }
            var purchase = _context.Purchases.Where(a => a.CourseId == course.Id && a.StudentId == student.Id).FirstOrDefault();

            if (purchase == null)
            {
                return NotFound("Student didn't buy this course yet....");
            }
            return Ok(new
            {
                courseContent = courseContent
            });

        }



        [HttpPost("Active")]
        public async Task<IActionResult> MakeCourseActive(int courseId)
        {
            var teaherEmail = HttpContext.Session.GetString("UserEmail");
            if (teaherEmail == null)
            {
                return Unauthorized("Teacher is not logged in.");
            }

            if (HttpContext.Session.GetString("type") != "Teacher")
            {
                return Unauthorized("A Teacher must be logged in to access this page");
            }

            var teacher = await _context.Teachers.Where(a => a.Email == teaherEmail).FirstOrDefaultAsync();
            if (teacher == null || !teacher.IsActive)
            {
                return Unauthorized("Teacher not authorized or inactive.");
            }

            var course = _context.Courses.Where(a => a.Id == courseId).FirstOrDefault();
            course.IsActive = true;
            _context.SaveChanges();
            return Ok(new { message = "Course content activeated successfully", courseId = course.Id });
        }


        // GET: api/teacher/view-course/{id}
        [HttpGet("view-course-student/{id}")]
        public async Task<IActionResult> ViewCourseStudent(int id)
        {
            

            var course = await _context.Courses
                .Include(c => c.CourseContents)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (course == null)
            {
                return NotFound("Course not found or you don't have permission to view it.");
            }

            var student = _context.Students.Where(a => a.Email == HttpContext.Session.GetString("UserEmail")).FirstOrDefault();
            if (student == null)
            {
                return NotFound("Student is not logged in...");
            }

            var purchase = _context.Purchases.Where(a => a.CourseId == id && a.StudentId == student.Id).FirstOrDefault();
            var purchased = true;
            if (purchase == null)
            {
                purchased = false;
            }

            return Ok(new
            {
                Id = course.Id,
                Title = course.Title,
                Description = course.Description,
                Price = course.Price,
                Picture = course.Picture,
                IsActive = course.IsActive,
                Contents = course.CourseContents.Select(cc => new CourseContentDto
                {
                    Id = cc.Id,
                    Title = cc.Title
                }).ToList(),
                Purchased = purchased
            });

        }


        //// POST: api/teacher/add-unit
        //[HttpPost("add-unit")]
        //public async Task<IActionResult> AddUnit([FromBody] UnitCreateDto dto)
        //{
        //    var teacherId = 1; // Hardcoded مؤقت
        //    var teacher = await _context.Teachers.FindAsync(teacherId);
        //    if (teacher == null)
        //    {
        //        return Unauthorized("Teacher not found. Please add a teacher first.");
        //    }
        //    if (!teacher.IsActive)
        //    {
        //        return Unauthorized("Teacher not activated.");
        //    }

        //    var course = await _context.Courses.FindAsync(dto.CourseId);
        //    if (course == null || course.TeacherId != teacherId || !course.IsActive)
        //    {
        //        return BadRequest("Invalid course ID, not owned by this teacher, or course is inactive.");
        //    }

        //    var unit = new Unit
        //    {
        //        Name = dto.Name,
        //        AdminId = null, // جعل AdminId null بدل 0
        //        Pictures = dto.Pictures,
        //        CourseId = dto.CourseId
        //    };

        //    _context.Units.Add(unit);
        //    await _context.SaveChangesAsync();

        //    return CreatedAtAction(nameof(GetMyUnits), new { id = unit.Id }, unit);
        //}

        // POST: api/teacher/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var teacher = await _context.Teachers.FirstOrDefaultAsync(t => t.Email == dto.Email);
            if (teacher == null)
            {
                return Unauthorized("Invalid email.");
            }

            //if (!BCrypt.Net.BCrypt.Verify(dto.Password, teacher.Password))
            //{
            //    return Unauthorized("Invalid password.");
            //}
            if (teacher.Password != dto.Password)
            {
                return Unauthorized("Invalid password.");
            }

            // تفعيل التيشر لأول مرة
            if (!teacher.IsActive)
            {
                teacher.IsActive = true;
                await _context.SaveChangesAsync();
            }

            HttpContext.Session.SetString("UserEmail", teacher.Email);
            HttpContext.Session.SetString("type", "Teacher");

            return Ok(new { message = "Login successful", teacherId = teacher.Id });
        }


            // GET: api/teacher/my-courses
            [HttpGet("my-courses")]
        public async Task<IActionResult> GetMyCourses()
        {
            var teaherEmail = HttpContext.Session.GetString("UserEmail");
            if (teaherEmail == null)
            {
                return Unauthorized("Teacher is not logged in.");
            }

            if (HttpContext.Session.GetString("type") != "Teacher")
            {
                return Unauthorized("A Teacher must be logged in to access this page");
            }

            var teacher = await _context.Teachers.Where(a => a.Email == teaherEmail).FirstOrDefaultAsync();
            if (teacher == null || !teacher.IsActive)
            {
                return Unauthorized("Teacher not authorized or inactive.");
            }


            // display all the courses that the teacher created even if it is not currently active
            var courses = await _context.Courses
                .Where(c => c.TeacherId == teacher.Id)
                .ToListAsync();

            return Ok(courses);
        }


        // Get all the purcheses for the courses of that teacher
        [HttpGet("my-sells")]
        public async Task<IActionResult> GetMySells()
        {
            var teacherEmail = HttpContext.Session.GetString("UserEmail");
            if (teacherEmail == null)
                return Unauthorized("Teacher is not logged in.");

            if (HttpContext.Session.GetString("type") != "Teacher")
                return Unauthorized("A Teacher must be logged in to access this page");

            var teacher = await _context.Teachers
                .FirstOrDefaultAsync(t => t.Email == teacherEmail && t.IsActive);

            if (teacher == null)
                return Unauthorized("Teacher not authorized or inactive.");

            // Get all Course IDs created by this teacher
            var courseIds = await _context.Courses
                .Where(c => c.TeacherId == teacher.Id && c.IsActive)
                .Select(c => c.Id)
                .ToListAsync();

            // Get all purchases for those courses
            var sells = await _context.Purchases
                .Where(p => courseIds.Contains(p.CourseId))
                .Select(p => new {
                    StudentName = p.Student.Name,
                    CourseTitle = p.Course.Title,
                    p.PurchaseDate
                })
                .ToListAsync();

            return Ok(sells);


        }

        




        //// GET: api/teacher/my-units
        //[HttpGet("my-units")]
        //public async Task<IActionResult> GetMyUnits()
        //{
        //    var teacherId = 1; // Hardcoded مؤقت
        //    var teacher = await _context.Teachers.FindAsync(teacherId);
        //    if (teacher == null || !teacher.IsActive)
        //    {
        //        return Unauthorized("Teacher not authorized or inactive.");
        //    }

        //    var units = await _context.Units
        //        .Where(u => u.CourseId != null && _context.Courses.Any(c => c.Id == u.CourseId && c.TeacherId == teacherId && c.IsActive))
        //        .ToListAsync();

        //    return Ok(units);
        //}
    }

    public class ActivateDto
    {
        public string Email { get; set; }
        public string Token { get; set; }
    }

    public class LoginDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class CourseCreateDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public IFormFile Picture { get; set; }

    }

    public class CourseUpdateDto
    {
        public int Id { get; set; }            // Course ID to update
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal? Price { get; set; }
        public IFormFile Picture { get; set; }

    }

    public class CourseViewDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string Picture { get; set; }
        public bool IsActive { get; set; }
        public List<string> ContentTitles { get; set; }
    }

    public class CourseContentDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
    }


    public class CourseContentCreateDto
    {
        public int? CourseId { get; set; }
        public string Title { get; set; }
        public IFormFile Content { get; set; }
    }

    public class CourseContentUpdateDto
    {
        public int? Id { get; set; }         
        public string Title { get; set; }
        public IFormFile Content { get; set; }    // New file to upload
    }


    public class UnitCreateDto
    {
        public string Name { get; set; }
        public List<string> Pictures { get; set; }
        public int CourseId { get; set; }
    }
}