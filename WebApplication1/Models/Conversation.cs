namespace WebApplication1.Models
{
    public class Conversation
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public string Pictures { get; set; }
        public int AdminId { get; set; }
        public Admin Admin { get; set; }
        // Navigation properties
        public ICollection<Message> Messages { get; set; }
        public ICollection<StudentConversation> StudentConversations { get; set; }
    }

}
