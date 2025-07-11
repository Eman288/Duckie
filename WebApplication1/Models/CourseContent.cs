using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace WebApplication1.Models
{
    public class CourseContent
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int? CourseId { get; set; }

        public string Title { get; set; }


        [Required]
        public string ContentUrl { get; set; }

        public Course? Course { get; set; }
    }
}