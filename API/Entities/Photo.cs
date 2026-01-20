using System.Text.Json.Serialization;

namespace API.Entities
{
    public class Photo
    {
        public int Id { get; set; }
        public required string Url { get; set; }
        
        // public id for could storage
        public string? PublicId { get; set; }


        // Navigation Property
        [JsonIgnore]
        // one member can have many photos => one-to-many relationship
        public Member Memebr { get; set; } = null!;
        public string MemberId { get; set; } = null!;

    }
}
