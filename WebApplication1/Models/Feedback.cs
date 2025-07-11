namespace WebApplication1.Models
{
    public class Feedback
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public int StarRate { get; set; }
        public int UnitId { get; set; }
        public Unit Unit { get; set; }

    }

}
