using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WebApplication1.Models
{
    public class StudentCourse
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int StudentId { get; set; }

        [Required]
        public int CourseId { get; set; }

        public DateTime PurchaseDate { get; set; } = DateTime.Now;

        public bool IsActive { get; set; } = true;

        [JsonIgnore]
        public Student Student { get; set; }

        [JsonIgnore]
        public Course Course { get; set; }
    }
}