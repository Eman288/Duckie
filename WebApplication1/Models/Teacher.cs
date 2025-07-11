using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations.Schema;


namespace WebApplication1.Models
{
    public class Teacher
    {
        [Key]
        public int Id { get; set; }


        public int AdminId { get; set; }


        [Required]
        public string Name { get; set; }

        public string Picture { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; } // Hashed Password

        [Column(TypeName = "decimal(10,2)")]
        public decimal Balance { get; set; }

        public bool IsActive { get; set; } = false; // يتفعّل في الـ Login

        [Required]
        public int CreatedBy { get; set; } // FK to Admins.Id

        [JsonIgnore]
        public Admin CreatedByAdmin { get; set; }

        [JsonIgnore]
        public ICollection<Course> Courses { get; set; }
    }
}