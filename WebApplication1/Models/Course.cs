using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace WebApplication1.Models
{
    public class Course
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int TeacherId { get; set; }

        [Required]
        public string Title { get; set; }

        public string Picture { get; set; }

        public string Description { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal Price { get; set; }


        public bool IsActive { get; set; } = true; // إضافة خاصية IsActive بقيمة افتراضية true

        [JsonIgnore]
        public Teacher Teacher { get; set; }
        [JsonIgnore]
        public ICollection<CourseContent> CourseContents { get; set; } 
    }
}