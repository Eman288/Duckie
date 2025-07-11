namespace WebApplication1.Models
{
    public class LessonContentViewModel
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public int UnitId { get; set; } // استخدم int بدلاً من string إذا كانت ID
    }
}
