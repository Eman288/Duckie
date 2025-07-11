namespace WebApplication1.Models
{
    
    public class Situation
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Pic { get; set; }
        public string Description { get; set; }
        public string VideoUrl { get; set; }
        public int AdminId { get; set; }
        public Admin Admin { get; set; }
        // Navigation properties
        public ICollection<StudentSituation> StudentSituations { get; set; }
    }
    
}
