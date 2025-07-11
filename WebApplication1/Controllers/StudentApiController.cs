using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using System;
//using WebApplication1.Data;
//using WebApplication1.Services;
using System.Text.Json;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BCrypt.Net;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudentApiController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public StudentApiController(AppDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            this._context = context;
            this._webHostEnvironment = webHostEnvironment;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] StudentRegistration model)
        {
            try
            {
                if (await _context.Students.AnyAsync(s => s.Email == model.Email))
                {
                    return BadRequest(new { message = "This email is already registered." });
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                string picturePath = null;
                if (model.Picture != null)
                {
                    var allowedExtensions = new[] { ".png", ".jpg", ".jpeg" };
                    var fileExtension = Path.GetExtension(model.Picture.FileName).ToLower();

                    if (!allowedExtensions.Contains(fileExtension))
                    {
                        return BadRequest(new { message = "Only PNG and JPG images are allowed." });
                    }

                    // Ensure the folder wwwroot/Images/Student exists
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "Images", "Student");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    // Save the file with a unique name
                    string uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
                    string fullPath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var fileStream = new FileStream(fullPath, FileMode.Create))
                    {
                        await model.Picture.CopyToAsync(fileStream);
                    }

                    // Save only the relative path for the image
                    picturePath = $"/Images/Student/{uniqueFileName}";
                }

                var student = new Student
                {
                    Name = model.Name,
                    Email = model.Email,
                    Password = BCrypt.Net.BCrypt.HashPassword(model.Password),
                    Birthdate = model.Birthdate,
                    JoinDate = DateTime.Now,
                    TotalPoints = -1,
                    DailyPoints = 0,
                    Picture = picturePath,
                    // Make sure these collections are initialized either here or in the model constructor
                    StudentUnits = new List<StudentUnit>(),
                    StudentLessons = new List<StudentLesson>(),
                    StudentSituations = new List<StudentSituation>()
                };

                _context.Students.Add(student);
                await _context.SaveChangesAsync(); // Save first to generate student.Id

                // Load all units and lessons from database
                var units = await _context.Units.ToListAsync();
                var lessons = await _context.Lessons.ToListAsync();
                var situations = await _context.Situations.ToListAsync();

                // Create StudentUnits
                foreach (var u in units)
                {
                    student.StudentUnits.Add(new StudentUnit
                    {
                        UnitId = u.Id,
                        StudentId = student.Id,
                        IsDone = false
                    });
                }

                // Create StudentLessons
                foreach (var l in lessons)
                {
                    student.StudentLessons.Add(new StudentLesson
                    {
                        LessonId = l.Id,
                        StudentId = student.Id,
                        IsDone = false
                    });
                }

                // Create StudentSituations (optional, as you had)
                foreach (var s in situations)
                {
                    student.StudentSituations.Add(new StudentSituation
                    {
                        SituationId = s.Id,
                        StudentId = student.Id,
                        IsWatched = false
                    });
                }

                // Save all changes again
                await _context.SaveChangesAsync();

                return Ok(new { message = "Registration successful!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred in Register: ", error = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromForm] StudentLogin model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return StatusCode(500, new { message = "An error occurred in Login: ", error = "Invalid email or password" });


                var student = await _context.Students.FirstOrDefaultAsync(s => s.Email == model.Email);
                if (student == null || !BCrypt.Net.BCrypt.Verify(model.Password, student.Password))
                {
                    return StatusCode(500, new { message = "An error occurred in Login: ", error = "Invalid email or password" });
                }

                HttpContext.Session.SetString("UserEmail", student.Email);
                HttpContext.Session.SetString("type", "Student");

                student.IsActive = true;
                _context.SaveChanges();

                return Ok(new { message = "Login Successful!", studentData = student});
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred in Login: ", error = ex.Message });
            }
        }

        [HttpPost("setlevel")]
        public async Task<IActionResult> SetLevel([FromQuery] string level)
        {
            var userEmail = HttpContext.Session.GetString("UserEmail");
            if (userEmail == null)
            {
                return BadRequest(new { message = "No user is Logged in" });
            }

            var stu = await _context.Students
                .Include(s => s.StudentUnits)
                .Include(s => s.StudentLessons)
                .FirstOrDefaultAsync(a => a.Email == userEmail);

            if (stu == null)
            {
                return BadRequest(new { message = "No user is Logged in" });
            }

            stu.level = level;
            stu.TotalPoints = 0;

            // Level order
            string[] levelOrder = new[] { "A1", "A2", "B1", "B2", "C1", "C2" };
            int currentLevelIndex = Array.IndexOf(levelOrder, level);
            if (currentLevelIndex == -1)
            {
                return BadRequest(new { message = "Invalid level" });
            }

            // Load units and lessons
            var unitsData = await _context.Units.ToListAsync();
            var lessonsData = await _context.Lessons
                .Include(l => l.Unit)
                .ToListAsync();

            // Order units and lessons in memory
            var allUnits = unitsData
                .OrderBy(u => Array.IndexOf(levelOrder, u.level))
                .ThenBy(u => u.OrderWithInLevel)
                .ToList();

            var allLessons = lessonsData
                .OrderBy(l => Array.IndexOf(levelOrder, l.Unit.level))
                .ThenBy(l => l.Unit.OrderWithInLevel)
                .ThenBy(l => l.OrderWithInLevel)
                .ToList();

            var unitsCurrentLevel = allUnits
                .Where(u => u.level == level)
                .OrderBy(u => u.OrderWithInLevel)
                .ToList();

            var firstUnitCurrentLevel = unitsCurrentLevel.FirstOrDefault();

            //  Update StudentUnits
            foreach (var su in stu.StudentUnits)
            {
                var unit = allUnits.FirstOrDefault(u => u.Id == su.UnitId);
                if (unit == null) continue;

                int unitLevelIndex = Array.IndexOf(levelOrder, unit.level);

                if (unitLevelIndex < currentLevelIndex) // All units before the current level
                {
                    su.IsDone = true;
                }
                else if (unitLevelIndex == currentLevelIndex) // Units in the current level
                {
                    su.IsDone = (unit.Id == firstUnitCurrentLevel?.Id);
                }
                else // Units after the current level
                {
                    su.IsDone = false;
                }
            }

            // ✅ Update StudentLessons
            foreach (var sl in stu.StudentLessons)
            {
                var lesson = allLessons.FirstOrDefault(l => l.Id == sl.LessonId);
                if (lesson == null) continue;

                int lessonLevelIndex = Array.IndexOf(levelOrder, lesson.Unit.level);

                if (lessonLevelIndex < currentLevelIndex) // All lessons before the current level
                {
                    sl.IsDone = true;
                }
                else if (lessonLevelIndex == currentLevelIndex) // Lessons in the current level
                {
                    if (lesson.UnitId == firstUnitCurrentLevel?.Id) // In the first unit
                    {
                        var lessonsInFirstUnit = allLessons
                            .Where(l => l.UnitId == firstUnitCurrentLevel.Id)
                            .OrderBy(l => l.OrderWithInLevel)
                            .ToList();

                        // First lesson = True, rest = False
                        sl.IsDone = (lesson.Id == lessonsInFirstUnit.FirstOrDefault()?.Id);
                    }
                    else
                    {
                        sl.IsDone = false;
                    }
                }
                else // Lessons after the current level
                {
                    sl.IsDone = false;
                }
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = "User level and progress updated successfully" });
        }


        // Create a DTO to receive parameters
        public class DashboardRequest
        {
            public string Type { get; set; }
        }

        //[HttpPost("purchase-course")]
        //public async Task<IActionResult> PurchaseCourse([FromBody] PurchaseRequest model)
        //{
        //    try
        //    {
        //        // التحقق من الطالب
        //        var student = await _context.Students.FindAsync(model.StudentId);
        //        if (student == null || !student.IsActive)
        //        {
        //            return Unauthorized(new { message = "Student not found or not activated." });
        //        }

        //        // التحقق من الكورس
        //        var course = await _context.Courses.FindAsync(model.CourseId);
        //        if (course == null || !course.IsActive)
        //        {
        //            return BadRequest(new { message = "Course not found or inactive." });
        //        }

        //        // التحقق إذا الطالب اشترى الكورس قبل كده
        //        var existingPurchase = await _context.Purchases
        //            .FirstOrDefaultAsync(p => p.StudentId == model.StudentId && p.CourseId == model.CourseId);
        //        if (existingPurchase != null)
        //        {
        //            return BadRequest(new { message = "Course already purchased by this student." });
        //        }

        //        // تحقق من وجود الملف و قراءته
        //        var jsonPath = Path.Combine(_webHostEnvironment.WebRootPath, "Data", "paymentMockData.json");
        //        if (!System.IO.File.Exists(jsonPath))
        //        {
        //            return StatusCode(500, new { message = $"Payment data file not found at: {jsonPath}" });
        //        }

        //        var jsonContent = await System.IO.File.ReadAllTextAsync(jsonPath);
        //        if (string.IsNullOrEmpty(jsonContent))
        //        {
        //            return StatusCode(500, new { message = "Payment data file is empty." });
        //        }

        //        var paymentData = System.Text.Json.JsonSerializer.Deserialize<List<PaymentCard>>(jsonContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        //        if (paymentData == null)
        //        {
        //            return StatusCode(500, new { message = "Failed to parse payment data. JSON content: " + jsonContent });
        //        }

        //        // تشخيص البيانات
        //        var card = paymentData.FirstOrDefault(c => c.CardNumber == model.CardNumber);
        //        if (card == null)
        //        {
        //            return BadRequest(new { message = $"Card {model.CardNumber} is not registered." });
        //        }
        //        if (!card.IsValid)
        //        {
        //            return BadRequest(new { message = $"Card {model.CardNumber} is not valid." });
        //        }
        //        if (string.IsNullOrEmpty(model.Cvv) || model.Cvv.Length != 3 || !int.TryParse(model.Cvv, out _))
        //        {
        //            return BadRequest(new { message = "Invalid CVV. Please enter a 3-digit number." });
        //        }
        //        if (string.IsNullOrEmpty(model.ExpireDate) || !DateTime.TryParse(model.ExpireDate, out DateTime expireDate) || expireDate < DateTime.Now)
        //        {
        //            return BadRequest(new { message = "Invalid or expired date. Please enter a valid future date (MM/YYYY)." });
        //        }

        //        // تسجيل الشراء
        //        var purchase = new Purchase
        //        {
        //            StudentId = model.StudentId,
        //            CourseId = model.CourseId,
        //            PurchaseDate = DateTime.UtcNow
        //        };

        //        _context.Purchases.Add(purchase);
        //        await _context.SaveChangesAsync();

        //        return Ok(new { message = "Payment successful! Course purchased.", purchaseId = purchase.Id });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new { message = "An error occurred during payment: ", error = ex.Message });
        //    }
        //}
        

        [HttpPost("dashboard")]
        public async Task<IActionResult> Dashboard([FromBody] DashboardRequest request)
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

                var student = _context.Students.FirstOrDefault(s => s.Email == userEmail);
                if (student == null)
                    return BadRequest(new { error = "Student not found" });

                // Constructing JSON response correctly
                var user = new { student.Id ,student.Name, student.TotalPoints, student.JoinDate, student.level, student.Picture };



                if (request.Type == "conversations")
                {
                   
                    return Ok(new { user, conversations = _context.Conversations.ToList() });
                }
                else if (request.Type == "situations")
                {
                    var situs = _context.Situations.ToList();
                    var stuSitIds = _context.StudentSituations
                    .Where(a => a.StudentId == student.Id && a.IsWatched)
                    .Select(a => a.SituationId)
                    .ToHashSet();


                    //var done = situs.Select(sit => stuSitIds.Contains(sit.Id) ? "Watched" : "Not Watched").ToList();

                    var enrichedSituations = situs.Select(sit => new {
                        sit.Id,
                        sit.Name,
                        sit.VideoUrl,
                        Watched = stuSitIds.Contains(sit.Id)
                    }).ToList();

                    return Ok(new { user, situations = enrichedSituations });

                }
                else if (request.Type == "courses")
                {
                    var courses = await _context.Courses
                        .Where(c => c.IsActive == true)
                        .ToListAsync();
                    return Ok(new { user, courses = courses });
                }
                else // Default case for "units"
                {
                    // Get StudentUnits for the current student
                    var studentUnitStatuses = _context.StudentUnits
                        .Where(su => su.StudentId == student.Id)
                        .ToList();

                    // Group units by level and attach IsDone status
                    var groupedUnits = _context.Units
                        .AsEnumerable() // Move to memory for shaping the object
                        .GroupBy(u => u.level)
                        .ToDictionary(g => g.Key, g => g.Select(u => new
                        {
                            u.Id,
                            u.Name,
                            u.Description,
                            u.OrderWithInLevel,
                            u.level,
                            u.Picture,
                            IsDone = studentUnitStatuses.Any(su => su.UnitId == u.Id && su.StudentId == student.Id && su.IsDone)
                        }).ToList());

                    return Ok(new { user, units = groupedUnits });
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
                return StatusCode(500, new { error = "An unexpected error occurred", details = ex.Message });
            }

        }




        [HttpGet("dashboard")]
        public IActionResult Dashboardt()
        {
            return RedirectToAction("Dashboard");
        }

        [HttpPost("profile")] // not tested
        public async Task<IActionResult> ShowProfile()
        {
            var userEmail = HttpContext.Session.GetString("UserEmail");
            if (string.IsNullOrEmpty(userEmail))
                return StatusCode(500, new { message = "An Error Occurred in ShowProfile: ", error = "No User is logged in" });

            var student = await _context.Students.FirstOrDefaultAsync(s => s.Email == userEmail);

            return Ok(new { message = "Profile Lodded!", user = student });
        }

        /*Convert to Api*/

        //[HttpGet]
        //public async Task<IActionResult> UpdateProfile()
        //{
        //    var userEmail = HttpContext.Session.GetString("UserEmail");
        //    if (string.IsNullOrEmpty(userEmail))
        //        return RedirectToAction("Login");

        //    var student = await _context.Students.FirstOrDefaultAsync(s => s.Email == userEmail);
        //    if (student == null)
        //        return RedirectToAction("Login");

        //    var model = new StudentUpdateProfileViewModel
        //    {
        //        Name = student.Name,
        //        Email = student.Email,
        //        Birthdate = student.Birthdate,
        //        ExistingPicturePath = student.Picture
        //    };

        //    return View(model);
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> UpdateProfile(StudentUpdateProfileViewModel model)
        //{
        //    var userEmail = HttpContext.Session.GetString("UserEmail");
        //    if (string.IsNullOrEmpty(userEmail))
        //        return RedirectToAction("Login");

        //    var student = await _context.Students.FirstOrDefaultAsync(s => s.Email == userEmail);
        //    if (student == null)
        //        return RedirectToAction("Login");

        //    if (!string.IsNullOrWhiteSpace(model.Name))
        //    {
        //        student.Name = model.Name;
        //    }

        //    if (!string.IsNullOrWhiteSpace(model.Email) &&
        //        !model.Email.Equals(student.Email, StringComparison.OrdinalIgnoreCase))
        //    {
        //        if (await _context.Students.AnyAsync(s => s.Email == model.Email))
        //        {
        //            ModelState.AddModelError("Email", "This email is already registered by another account.");
        //            model.ExistingPicturePath = student.Picture;
        //            return View(model);
        //        }
        //        student.Email = model.Email;
        //        HttpContext.Session.SetString("UserEmail", student.Email);
        //    }

        //    if (model.Birthdate.HasValue)
        //    {
        //        student.Birthdate = model.Birthdate.Value;
        //    }

        //    if (model.Picture != null)
        //    {
        //        var allowedExtensions = new[] { ".png", ".jpg", ".jpeg" };
        //        var fileExtension = Path.GetExtension(model.Picture.FileName).ToLower();

        //        if (!allowedExtensions.Contains(fileExtension))
        //        {
        //            ModelState.AddModelError("Picture", "Only PNG and JPG images are allowed.");
        //            model.ExistingPicturePath = student.Picture;
        //            return View(model);
        //        }

        //        if (!string.IsNullOrEmpty(student.Picture) && System.IO.File.Exists(student.Picture))
        //        {
        //            System.IO.File.Delete(student.Picture);
        //        }

        //        string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
        //        Directory.CreateDirectory(uploadsFolder);

        //        string uniqueFileName = $"{Path.GetFileNameWithoutExtension(model.Picture.FileName)}_{Guid.NewGuid()}{fileExtension}";
        //        string newPicturePath = Path.Combine(uploadsFolder, uniqueFileName);

        //        using (var fileStream = new FileStream(newPicturePath, FileMode.Create))
        //        {
        //            await model.Picture.CopyToAsync(fileStream);
        //        }
        //        student.Picture = newPicturePath;
        //    }



        //    await _context.SaveChangesAsync();

        //    ViewBag.SuccessMessage = "The update is complete.";
        //    return View(model);
        //}


        // Logout Method
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            Response.Cookies.Delete(".AspNetCore.Session");
            return Ok(new { message = "User Logged Out!" });
        }

//        // Forgot Password Method
//        [HttpPost("forgotPass")]
//        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
//        {
//            try
//            {
//                var student = await _context.Students.FirstOrDefaultAsync(s => s.Email == request.Email);
//                if (student == null)
//                {
//                    return Ok(new { message = "If the email exists, a reset link has been sent." });
//                }

//                var token = Guid.NewGuid().ToString();
//                var resetToken = new PasswordResetToken
//                {
//                    Email = request.Email,
//                    Token = token,
//                    ExpiryDate = DateTime.Now.AddHours(1)
//                };

//                _context.PasswordResetTokens.Add(resetToken);
//                await _context.SaveChangesAsync();

//                var resetLink = $"http://localhost:3000/reset-password?token={token}&email={request.Email}";
//                var emailBody = $@"
//<!DOCTYPE html>
//<html>
//<head>
//    <style>
//        body {{ font-family: Arial, sans-serif; color: #333; line-height: 1.6; }}
//        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; background-color: #f9f9f9; border-radius: 8px; }}
//        .header {{ background-color: #4CAF50; color: white; padding: 15px; text-align: center; border-radius: 8px 8px 0 0; }}
//        .content {{ background-color: white; padding: 20px; border-radius: 0 0 8px 8px; }}
//        .button {{ display: inline-block; padding: 12px 24px; background-color: #4CAF50; color: white; text-decoration: none; border-radius: 5px; }}
//        .button:hover {{ background-color: #45a049; }}
//        .footer {{ text-align: center; font-size: 12px; color: #777; margin-top: 20px; }}
//    </style>
//</head>
//<body>
//    <div class='container'>
//        <div class='header'>
//            <h2>Welcome to Ducki! 🦆</h2>
//        </div>
//        <div class='content'>
//            <p>Dear {student.Name},</p>
//            <p>We heard you need to reset your password for your Ducki account. No worries, we’ve got you covered!</p>
//            <p>Click the button below to set a new password and get back to learning with Ducki:</p>
//            <p style='text-align: center;'>
//                <a href='{resetLink}' class='button'>Reset Your Password</a>
//            </p>
//            <p>This link will expire in <strong>1 hour</strong> for your security.</p>
//            <p>If you didn’t request this, you can safely ignore this email—your account is secure.</p>
//            <p>Happy learning!</p>
//            <p>Best regards,<br>Ahmed Aziz<br>Creator from Ducki 🦆</p>
//        </div>
//        <div class='footer'>
//            <p>© 2025 Ducki. All rights reserved.</p>
//        </div>
//    </div>
//</body>
//</html>";

//                await _emailService.SendEmailAsync(request.Email, "Password Reset Request", emailBody);

//                return Ok(new { message = "If the email exists, a reset link has been sent." });
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, new { message = "An error occurred in ForgotPassword: ", error = ex.Message });
//            }
//        }

//        // Reset Password Method
//        [HttpPost("resetPass")]
//        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
//        {
//            try
//            {
//                var resetToken = await _context.PasswordResetTokens
//                    .FirstOrDefaultAsync(t => t.Email == request.Email && t.Token == request.Token && t.ExpiryDate > DateTime.Now);

//                if (resetToken == null)
//                {
//                    return BadRequest(new { message = "Invalid or expired token." });
//                }

//                var student = await _context.Students.FirstOrDefaultAsync(s => s.Email == request.Email);
//                if (student == null)
//                {
//                    return BadRequest(new { message = "Email not found." });
//                }

//                if (string.IsNullOrEmpty(request.Password) || request.Password.Length < 8)
//                {
//                    return BadRequest(new { message = "Password must be at least 8 characters long." });
//                }

//                student.Password = BCrypt.Net.BCrypt.HashPassword(request.Password);
//                _context.Students.Update(student);

//                _context.PasswordResetTokens.Remove(resetToken);
//                await _context.SaveChangesAsync();

//                return Ok(new { message = "Password reset successfully!" });
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, new { message = "An error occurred in ResetPassword: ", error = ex.Message });
//            }
//        }

        // GET: api/student/leaderboard
        // GET: api/student/leaderboard
        // GET: api/StudentApi/leaderboard
        [HttpGet("leaderboard")]
        public async Task<IActionResult> GetStudentLeaderboard()
        {
            var students = await _context.Students
                .OrderByDescending(s => s.TotalPoints) // ترتيب تنازلي حسب TotalPoints
                .ToListAsync(); // جيب البيانات للـ Client

            var rankedStudents = students.Select((s, index) => new
            {
                Id = s.Id,
                Name = s.Name,
                TotalPoints = s.TotalPoints,
                Rank = index + 1 // تعيين الرتبة هنا
            }).ToList();

            return Ok(rankedStudents);
        }

        [HttpPost("purchase-course")]
        public async Task<IActionResult> PurchaseCourse([FromBody] PurchaseRequest model)
        {
            try
            {
                // Get student from session
                var student = await _context.Students
                    .FirstOrDefaultAsync(s => s.Email == HttpContext.Session.GetString("UserEmail"));
                if (student == null || !student.IsActive)
                {
                    return Unauthorized(new { message = "Student not found or not activated." });
                }

                // Check course
                var course = await _context.Courses.FindAsync(model.CourseId);
                if (course == null || !course.IsActive)
                {
                    return BadRequest(new { message = "Course not found or inactive." });
                }

                //  Double-check the price from DB
                if (course.Price != model.Price)
                {
                    return BadRequest(new { message = "Price mismatch. Please refresh the page." });
                }

                // Check if already purchased
                var existingPurchase = await _context.Purchases
                    .FirstOrDefaultAsync(p => p.StudentId == student.Id && p.CourseId == model.CourseId);
                if (existingPurchase != null)
                {
                    return BadRequest(new { message = "Course already purchased." });
                }

                // Read payment mock data
                var jsonPath = Path.Combine(_webHostEnvironment.WebRootPath, "Data", "paymentMockData.json");
                if (!System.IO.File.Exists(jsonPath))
                {
                    return StatusCode(500, new { message = $"Payment data file not found at: {jsonPath}" });
                }

                var jsonContent = await System.IO.File.ReadAllTextAsync(jsonPath);
                var paymentData = System.Text.Json.JsonSerializer.Deserialize<List<PaymentCard>>(jsonContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if (paymentData == null)
                {
                    return StatusCode(500, new { message = "Failed to parse payment data." });
                }

                // Check card
                var card = paymentData.FirstOrDefault(c => c.CardNumber == model.CardNumber);
                if (card == null)
                {
                    return BadRequest(new { message = $"Card {model.CardNumber} is not registered." });
                }
                if (!card.IsValid)
                {
                    return BadRequest(new { message = $"Card {model.CardNumber} is not valid." });
                }
                if (string.IsNullOrEmpty(model.Cvv) || model.Cvv.Length != 3 || !int.TryParse(model.Cvv, out _))
                {
                    return BadRequest(new { message = "Invalid CVV. Please enter a 3-digit number." });
                }
                if (string.IsNullOrEmpty(model.ExpireDate) || !DateTime.TryParse(model.ExpireDate, out DateTime expireDate) || expireDate < DateTime.Now)
                {
                    return BadRequest(new { message = "Invalid or expired date. Please enter a valid future date (MM/YYYY)." });
                }

                // Save purchase
                var purchase = new Purchase
                {
                    StudentId = student.Id,
                    CourseId = model.CourseId,
                    PurchaseDate = DateTime.UtcNow
                };

                _context.Purchases.Add(purchase);
                // find the teacher and add the price to their balance - 10%
                var teacher = _context.Teachers.Where(a => a.Id == course.TeacherId).FirstOrDefault();
                if (teacher.Balance == null)
                {
                    teacher.Balance = 0;
                }
                teacher.Balance += course.Price - (course.Price * 0.1m); // 10% for duckie
                await _context.SaveChangesAsync();


                return Ok(new { message = "Payment successful! Course purchased.", purchaseId = purchase.Id });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred during payment.", error = ex.Message });
            }
        }

        // Helper method for MM/YYYY parsing
        private bool TryParseExpireDate(string expireDateStr, out DateTime expireDate)
        {
            expireDate = DateTime.MinValue;
            var parts = expireDateStr.Split('/');
            if (parts.Length != 2) return false;

            if (!int.TryParse(parts[0], out int month) || !int.TryParse(parts[1], out int year))
                return false;

            if (year < 100) year += 2000; // if year like 25 -> 2025

            try
            {
                // Set to end of the month
                expireDate = new DateTime(year, month, DateTime.DaysInMonth(year, month));
                return true;
            }
            catch
            {
                return false;
            }
        }

        //// Request Models
        //public class PurchaseRequest
        //{
        //    public int StudentId { get; set; }
        //    public int CourseId { get; set; }
        //    public string CardNumber { get; set; }
        //    public string Cvv { get; set; } // CVV
        //    public string ExpireDate { get; set; } // تاريخ الانتهاء
        //}

        //public class PaymentCard
        //{
        //    public string CardNumber { get; set; }
        //    public bool IsValid { get; set; }
        //}
        //// Request Models
        //public class ForgotPasswordRequest
        //{
        //    [Required]
        //    [EmailAddress]
        //    public string Email { get; set; }
        //}

        //public class ResetPasswordRequest
        //{
        //    [Required]
        //    [EmailAddress]
        //    public string Email { get; set; }
        //    [Required]
        //    public string Token { get; set; }
        //    [Required]
        //    [StringLength(100, MinimumLength = 8)]
        //    public string Password { get; set; }
        //}



        public class PurchaseRequest
        {
            public int CourseId { get; set; }
            public string CardNumber { get; set; }
            public string Cvv { get; set; }
            public string ExpireDate { get; set; }
            public decimal Price { get; set; }
        }


        public class PaymentCard
        {
            public string CardNumber { get; set; }
            public bool IsValid { get; set; }
        }

    }
}
