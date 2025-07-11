using Microsoft.VisualBasic;
using WebApplication1.Models;

public class Admin
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }

    public ICollection<Unit> Units { get; set; }
    public ICollection<Teacher> Teachers { get; set; }

    //public ICollection<Word> Words { get; set; }
    public ICollection<Lesson> Lessons { get; set; }
    //public ICollection<Quiz> Quizzes { get; set; }
    //public ICollection<Achievement> Achievements { get; set; }
    //public ICollection<Question> Questions { get; set; }
    //public ICollection<Answer> Answers { get; set; }
    public ICollection<Conversation> Conversations { get; set; }
}
