namespace WebApplication1.Models
{
    public class Answer
    {
        public int Id { get; set; } 
        public string Content { get; set; }
        public bool IsCorrect { get; set; }  // ✅ To mark correct answers

        // ✅ Foreign Key for Question
        public int QuestionId { get; set; }
        public Question Question { get; set; }

    }

}
