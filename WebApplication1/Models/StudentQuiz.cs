namespace WebApplication1.Models
{
    public class StudentQuiz
    {
        public int StudentId { get; set; }
        public Student Student { get; set; }
        public int QuizId { get; set; }
        public Quiz Quiz { get; set; }
        public bool isDone = false;
    }
}
