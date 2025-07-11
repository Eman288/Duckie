namespace WebApplication1.Models
{
    public class StudentAchievement
    {
        public int StudentId { get; set; }
        public Student Student { get; set; }

        public int AchievementId { get; set; }
        public Achievement Achievement { get; set; }
    }
}
