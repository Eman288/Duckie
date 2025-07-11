using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    public class APITestController : Controller
    {

        [HttpGet("text")] // Now the API route is: /api/home/text
        public IActionResult TryApi()
        {
            var data = new { message = "Your MVC project is now an API!" };
            return Ok(data); // Returns JSON
        }
    }
}
