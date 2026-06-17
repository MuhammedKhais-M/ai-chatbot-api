namespace AIChatBot.API.Models
{
    public class Message
    {
        public int Id { get; set; }

        public int ConversationId { get; set; }

        public string? Role { get; set; }

        public string? Content { get; set; }
    }
}
