using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class PasswordResetToken
    {
        public int Id { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Token { get; set; }
        [Required]
        public DateTime ExpiryDate { get; set; }
    }
}