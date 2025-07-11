using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class Student
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(50, ErrorMessage = "Name cannot exceed 50 characters.")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters long.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required(ErrorMessage = "Birthdate is required.")]
        public DateTime Birthdate { get; set; }
        public string level { get; set; } = "A1";
        public DateTime JoinDate { get; set; }
        public int TotalPoints { get; set; } = -1;
        public int DailyPoints { get; set; }
        public string Picture { get; set; }
        public bool IsActive { get; set; } = false; // إضافة هنا

        // public ICollection<StudentUnit> StudentUnit { get; set; }
        public ICollection<StudentLesson> StudentLessons { get; set; } = new List<StudentLesson>();
        public ICollection<StudentUnit> StudentUnits { get; set; } = new List<StudentUnit>();

        public ICollection<Purchase> Purchases { get; set; } = new List<Purchase>();
        public ICollection<StudentAchievement> StudentAchievements { get; set; } = new List<StudentAchievement>();
        public ICollection<StudentConversation> StudentConversations { get; set; } = new List<StudentConversation>();
        public ICollection<StudentQuiz> StudentQuizzes { get; set; } = new List<StudentQuiz>();
        public ICollection<StudentSituation> StudentSituations { get; set; } = new List<StudentSituation>();

    }
}
