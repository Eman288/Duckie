using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations;

public class CreateUnitDto
{
    [Required]
    public string Name { get; set; }

    [Required]
    public string Level { get; set; }

    public IFormFile QuizFile { get; set; }

    [Required]
    public string Quiz { get; set; }

    [Required]
    public string Description { get; set; }

    [Required]
    public int OrderWithInLevel { get; set; }

    public IFormFile UnitImageFile { get; set; }

    [Required]
    public int AdminId { get; set; }

 
}

public class PictureValidationAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var pictures = value as List<string>;
        if (pictures == null)
            return ValidationResult.Success;

        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };

        foreach (var pic in pictures)
        {
            var extension = System.IO.Path.GetExtension(pic)?.ToLower();
            if (!allowedExtensions.Contains(extension))
                return new ValidationResult(ErrorMessage);
        }

        return ValidationResult.Success;
    }
}
