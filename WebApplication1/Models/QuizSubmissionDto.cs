public class QuizSubmissionDto
{
    public int StudentId { get; set; }
    public Dictionary<int, string> Answers { get; set; } // {QuizId: Answer}
}
