using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace WebApplication1.Models
{
    public class StudentUpdateProfileViewModel
    {
        [StringLength(50, ErrorMessage = "Name cannot exceed 50 characters.")]
        public string Name { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Birthdate is required.")]
        [DataType(DataType.Date)]
        public DateTime? Birthdate { get; set; }

        public IFormFile? Picture { get; set; }

        public string? ExistingPicturePath { get; set; }

   }

}
