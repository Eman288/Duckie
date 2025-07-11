namespace WebApplication1.Models
{
    public class Quiz
    {
        public int Id { get; set; }
        public string Name { get; set; }

        //  public int AdminId { get; set; }
        //  public Admin Admin { get; set; }
        public int UnitId { get; set; }
        public Unit Unit { get; set; }
        public int Lesson { get; set; } = 1;

        public ICollection<Question> Questions { get; set; } = new List<Question>();
        public ICollection<StudentQuiz> StudentQuizzes { get; set; } = new List<StudentQuiz>();
    }
}
