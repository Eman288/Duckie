namespace WebApplication1.Models
{
    public class StudentProfileViewModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string JoinDate { get; set; }
        public int TotalPoints { get; set; }
        public string ProfilePicture { get; set; }
        public DateTime? Birthdate { get; set; }
     //   public List<Unit> Units { get; set; }  // Add units here
        public List<UnitViewModel> Units { get; set; }


    }
    public class UnitViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
