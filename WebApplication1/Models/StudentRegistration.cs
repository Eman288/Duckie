using System.ComponentModel.DataAnnotations;
using BCrypt.Net;

namespace WebApplication1.Models
{
    public class StudentRegistration
    {
       [ Required]
    [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; }

        [Required]
        public DateTime Birthdate { get; set; }

        public IFormFile Picture { get; set; }

        public void HashPassword()
        {
            if (!string.IsNullOrEmpty(Password))
            {
                Password = BCrypt.Net.BCrypt.HashPassword(Password);
            }
        }
    }
}
