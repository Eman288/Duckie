using System.ComponentModel.DataAnnotations;

public class CreateLessonDto
{
    [Required]
    public string Name { get; set; }

    [Required]
    [RegularExpression(@"^.*\.(jpg|png)$", ErrorMessage = "Only .jpg or .png images are allowed.")]
    public string Pictures { get; set; }

    [Required]
    public int UnitId { get; set; }

    [Required]
    public int AdminId { get; set; }

    [Required]
    public IFormFile Content { get; set; }

    [Required]
    public IFormFile Quiz { get; set; }
}
