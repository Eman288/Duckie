using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace WebApplication1.Models
{
    public class Lesson
    {
        [Key]
    
        public int Id { get; set; }
        public string Name { get; set; }
        public string Pictures { get; set; }
        public int UnitId { get; set; }
        public Unit Unit { get; set; }
        public bool locked { get; set; } = false;
        public int OrderWithInLevel { get; set; }
        public string quizContent { get; set; }
        public string content { get; set; }
        public int AdminId { get; set; }
        public Admin Admin { get; set; }
        public ICollection<Question> Questions { get; set; } = new List<Question>();
        public ICollection<StudentLesson> StudentLessons { get; set; } = new List<StudentLesson>();
    }
}
