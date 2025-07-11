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
    public class ConversationApiController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ConversationApiController(AppDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet("index")]
        public IActionResult Index()
        {
            return Ok(new { message = "conversations ok!" });
        }
    }
}
