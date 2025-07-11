using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class PlacementQuiz
    {
        public int Id { get; set; }

        [Required]
        public string Question { get; set; }

        [Required]
        public string OptionA { get; set; }

        [Required]
        public string OptionB { get; set; }

        [Required]
        public string OptionC { get; set; }

        [Required]
        public string OptionD { get; set; }

        [Required]
        public string CorrectAnswer { get; set; }
    }
}
