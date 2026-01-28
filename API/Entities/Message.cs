namespace API.Entities
{
    public class Message
    {
        // auto-incrementing id
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public required string Content { get; set; }
        public DateTime? DateRead { get; set; }
        public DateTime MessageSent { get; set; } = DateTime.UtcNow;
        // if sender has deleted the message
        public bool SenderDeleted { get; set; }

        // if recepient has deleted the message
        public bool RecipientDeleted { get; set; }

        // nav properties 
        public required string SenderId { get; set; }
        public Member Sender { get; set; } = null!;

        public required string RecipientId { get; set; }
        public Member Recipient { get; set; } = null!;


    }
}
