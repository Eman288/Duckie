using System.ComponentModel.DataAnnotations;

public class LessonDisplayRequestDto
{
    public int UnitId { get; set; }
}

public class LessonDto
{
    public int Id { get; set; }
    public string Name { get; set; }
}
