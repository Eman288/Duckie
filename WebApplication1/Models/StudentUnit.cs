namespace WebApplication1.Models
{
    public class StudentUnit
    {
        public int StudentId { get; set; }
        public Student Student { get; set; }
        public int UnitId { get; set; }
        public Unit Unit { get; set; }
        public bool IsDone { get; set; } = false;

    }
}
