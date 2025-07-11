using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Admin> Admins { get; set; }
    public DbSet<Student> Students { get; set; }
    public DbSet<Unit> Units { get; set; }
    public DbSet<Lesson> Lessons { get; set; }
    public DbSet<Quiz> Quizzes { get; set; }
    public DbSet<Question> Questions { get; set; }
    public DbSet<Answer> Answers { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<Conversation> Conversations { get; set; }
    public DbSet<Achievement> Achievements { get; set; }
    public DbSet<Feedback> Feedbacks { get; set; }
    public DbSet<Situation> Situations { get; set; }
    public DbSet<PlacementQuiz> PlacementQuizzes { get; set; }
    public DbSet<Teacher> Teachers { get; set; }
    public DbSet<Course> Courses { get; set; }
    public DbSet<StudentCourse> StudentCourses { get; set; }
    public DbSet<CourseContent> CourseContents { get; set; }
    public DbSet<StudentAchievement> StudentAchievements { get; set; }
    public DbSet<StudentLesson> StudentLessons { get; set; }
    public DbSet<StudentUnit> StudentUnits { get; set; }
    public DbSet<StudentQuiz> StudentQuizzes { get; set; }
    public DbSet<StudentConversation> StudentConversations { get; set; }
    public DbSet<StudentSituation> StudentSituations { get; set; }
    public DbSet<Purchase> Purchases { get; set; }
    public DbSet<PasswordResetToken> PasswordResetTokens { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Unit>()
            .HasOne(u => u.Admin)
            .WithMany(a => a.Units)
            .HasForeignKey(u => u.AdminId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<StudentUnit>().HasKey(su => new { su.StudentId, su.UnitId });
        modelBuilder.Entity<StudentUnit>()
            .HasOne(su => su.Student)
            .WithMany(s => s.StudentUnits)
            .HasForeignKey(su => su.StudentId);
        modelBuilder.Entity<StudentUnit>()
            .HasOne(su => su.Unit)
            .WithMany(u => u.StudentUnits)
            .HasForeignKey(su => su.UnitId);

        modelBuilder.Entity<StudentAchievement>().HasKey(sa => new { sa.StudentId, sa.AchievementId });
        modelBuilder.Entity<StudentAchievement>()
            .HasOne(sa => sa.Student)
            .WithMany(s => s.StudentAchievements)
            .HasForeignKey(sa => sa.StudentId);
        modelBuilder.Entity<StudentAchievement>()
            .HasOne(sa => sa.Achievement)
            .WithMany(a => a.StudentAchievements)
            .HasForeignKey(sa => sa.AchievementId);

        modelBuilder.Entity<StudentLesson>().HasKey(sl => new { sl.StudentId, sl.LessonId });
        modelBuilder.Entity<StudentLesson>()
            .HasOne(sl => sl.Student)
            .WithMany(s => s.StudentLessons)
            .HasForeignKey(sl => sl.StudentId);
        modelBuilder.Entity<StudentLesson>()
            .HasOne(sl => sl.Lesson)
            .WithMany(l => l.StudentLessons)
            .HasForeignKey(sl => sl.LessonId);

        modelBuilder.Entity<StudentQuiz>().HasKey(sq => new { sq.StudentId, sq.QuizId });
        modelBuilder.Entity<StudentQuiz>()
            .HasOne(sq => sq.Student)
            .WithMany(s => s.StudentQuizzes)
            .HasForeignKey(sq => sq.StudentId);
        modelBuilder.Entity<StudentQuiz>()
            .HasOne(sq => sq.Quiz)
            .WithMany(q => q.StudentQuizzes)
            .HasForeignKey(sq => sq.QuizId);

        modelBuilder.Entity<StudentConversation>().HasKey(sc => new { sc.StudentId, sc.ConversationId });
        modelBuilder.Entity<StudentConversation>()
            .HasOne(sc => sc.Student)
            .WithMany(s => s.StudentConversations)
            .HasForeignKey(sc => sc.StudentId);
        modelBuilder.Entity<StudentConversation>()
            .HasOne(sc => sc.Conversation)
            .WithMany(c => c.StudentConversations)
            .HasForeignKey(sc => sc.ConversationId);

        modelBuilder.Entity<StudentSituation>().HasKey(ss => new { ss.StudentId, ss.SituationId });
        modelBuilder.Entity<StudentSituation>()
            .HasOne(ss => ss.Student)
            .WithMany(s => s.StudentSituations)
            .HasForeignKey(ss => ss.StudentId);
        modelBuilder.Entity<StudentSituation>()
            .HasOne(ss => ss.Situation)
            .WithMany(s => s.StudentSituations)
            .HasForeignKey(ss => ss.SituationId);

        modelBuilder.Entity<Message>()
            .HasOne(m => m.Conversation)
            .WithMany(c => c.Messages)
            .HasForeignKey(m => m.ConversationId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Question>()
            .HasOne(q => q.Quiz)
            .WithMany(qz => qz.Questions)
            .HasForeignKey(q => q.QuizId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Answer>()
            .HasOne(a => a.Question)
            .WithMany(q => q.Answers)
            .HasForeignKey(a => a.QuestionId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Question>()
            .HasOne(q => q.Lesson)
            .WithMany(l => l.Questions)
            .HasForeignKey(q => q.LessonId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Lesson>()
            .HasOne(l => l.Unit)
            .WithMany(u => u.Lessons)
            .HasForeignKey(l => l.UnitId)
            .OnDelete(DeleteBehavior.Cascade);


        // Teacher - CreatedBy
        modelBuilder.Entity<Teacher>()
            .HasOne(t => t.CreatedByAdmin)
            .WithMany(a => a.Teachers)
            .HasForeignKey(t => t.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict);

        // Course - Teacher
        modelBuilder.Entity<Course>()
            .HasOne(c => c.Teacher)
            .WithMany(t => t.Courses)
            .HasForeignKey(c => c.TeacherId)
            .OnDelete(DeleteBehavior.Cascade);

        // StudentCourse
        modelBuilder.Entity<StudentCourse>().HasKey(sc => new { sc.StudentId, sc.CourseId });

        modelBuilder.Entity<StudentCourse>()
            .HasOne(sc => sc.Student)
            .WithMany()
            .HasForeignKey(sc => sc.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<StudentCourse>()
            .HasOne(sc => sc.Course)
            .WithMany()
            .HasForeignKey(sc => sc.CourseId)
            .OnDelete(DeleteBehavior.Restrict);

        // CourseContent - Course
        modelBuilder.Entity<CourseContent>()
            .HasOne(u => u.Course)
            .WithMany(c => c.CourseContents)
            .HasForeignKey(u => u.CourseId)
            .OnDelete(DeleteBehavior.SetNull);

        // Purchase
        modelBuilder.Entity<Purchase>()
            .HasOne(p => p.Student)
            .WithMany(s => s.Purchases)
            .HasForeignKey(p => p.StudentId);

        modelBuilder.Entity<Purchase>()
            .HasOne(p => p.Course)
            .WithMany()
            .HasForeignKey(p => p.CourseId);

    }
}
