namespace WebApplication1.Models
{
    public class Achievement
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<string> Pictures { get; set; } = new List<string>();
        public string Description { get; set; }
        public int PointsNeeded { get; set; }

        public ICollection<StudentAchievement> StudentAchievements { get; set; } = new HashSet<StudentAchievement>(); // (M ↔ M)
    }

}
