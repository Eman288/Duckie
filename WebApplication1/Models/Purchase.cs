using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations.Schema;
namespace WebApplication1.Models
{
    public class Purchase
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int StudentId { get; set; }

        [Required]
        public int CourseId { get; set; }

        public DateTime PurchaseDate { get; set; } = DateTime.UtcNow;

        [Column(TypeName = "decimal(10,2)")]
        public decimal TotalPrice { get; set; }

        [JsonIgnore]
        public Student Student { get; set; }

        [JsonIgnore]
        public Course Course { get; set; }
    }
}