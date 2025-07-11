using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    public class HomeApiController : Controller
    {
        private readonly ILogger<HomeApiController> _logger;
        private readonly AppDbContext _context;

        public HomeApiController(ILogger<HomeApiController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        //[HttpPost("index")]
        //public async Task<IActionResult> Index()
        //{
        //    var user = await _context.Students.Where(a => a.Email == HttpContext.Session.GetString("UserEmail")).FirstOrDefaultAsync();
        //    return Ok(user.Email);
        //}

        //public IActionResult Login()
        //{
        //    return View();
        //}
        //public IActionResult Index()
        //{
        //    ViewBag.UserEmail = HttpContext.Session.GetString("UserEmail");

        //    return View();
        //}

        //public IActionResult Privacy()
        //{
        //    return View();
        //}
        //public IActionResult Register()
        //{
        //    return RedirectToAction("Register", "Student");
        //}
        //[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        //public IActionResult Error()
        //{
        //    return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        //}
    }
}