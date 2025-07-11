namespace WebApplication1.Models
{
    public class Message
    {
        public int Id { get; set; }
        public int UserId { get; set; }

        public string Content { get; set; }
        public DateTime Date { get; set; }

        // Foreign key
        public int ConversationId { get; set; }

        public Conversation Conversation { get; set; }

    }
}
