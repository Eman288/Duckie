using WebApplication1.Models;

public class Question
{
    public int Id { get; set; }
    public string Content { get; set; }

    public ICollection<Answer> Answers { get; set; }

    public int QuizId { get; set; }
    public Quiz Quiz { get; set; }

    // Foreign Key for Lesson
    public int LessonId { get; set; }
    public Lesson Lesson { get; set; }


    // public int AdminId { get; set; }
    // public Admin Admin { get; set; }
}
