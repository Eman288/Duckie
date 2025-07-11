namespace WebApplication1.Models
{
    public class StudentSituation
    {
        public int StudentId { get; set; }
        public Student Student { get; set; }
        public int SituationId { get; set; }
        public Situation Situation { get; set; }
        public bool IsWatched { get; set; }


    }
}
