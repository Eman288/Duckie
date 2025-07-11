namespace WebApplication1.Models
{
    
        public class StudentDashboardViewModel
        {
            public List<Unit> Units { get; set; } = new List<Unit>();  // قائمة بالـ Units
            public Unit? SelectedUnit { get; set; }  // الوحدة المحددة
            public List<Lesson> Lessons { get; set; } = new List<Lesson>();  // الدروس التابعة للوحدة المحددة
       
    }
}
