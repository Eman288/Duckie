using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class Unit
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string level { get; set; }
        public string Description { get; set; }
        public int OrderWithInLevel { get; set; }
        public bool locked { get; set; } = false;
        public int AdminId { get; set; }
        public Admin Admin { get; set; }
        public string Quiz { get; set; }
        public string Picture { get; set; }


        public ICollection<Lesson> Lessons { get; set; } = new HashSet<Lesson>();
        public ICollection<Feedback> Feedbacks { get; set; } = new HashSet<Feedback>();
        public ICollection<StudentUnit> StudentUnits { get; set; } = new List<StudentUnit>();
    }
}
