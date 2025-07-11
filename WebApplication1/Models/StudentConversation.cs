namespace WebApplication1.Models
{
    public class StudentConversation
    {
        public int StudentId { get; set; }
        public Student Student { get; set; }

        public int ConversationId { get; set; }
        public Conversation Conversation { get; set; }
    }

}
