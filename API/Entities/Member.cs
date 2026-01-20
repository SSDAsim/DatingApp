using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace API.Entities
{
    public class Member
    {
        public string Id { get; set; } = null!;
        public DateOnly DateOfBirth {  get; set; }
        public string? ImageUrl { get; set; }
        public required string DisplayName { get; set; }
        public DateTime Created { get; set; } = DateTime.UtcNow;
        public DateTime LastActive { get; set; } = DateTime.UtcNow;
        public required string Gender { get; set; }
        public string? Description { get; set; }
        public required string City { get; set;  }
        public required string Country { get; set;  }

        // Navigation Property
        // Navigtion Property for Photo
        [JsonIgnore] // ignore this when preparing response data to return from API
        public List<Photo> Photos { get; set; } = [];

        [JsonIgnore]
        [ForeignKey(nameof(Id))]
        // The property which is going to relate Member to the AppUser. From Member, we will be able to access AppUser
        public AppUser User { get; set; } = null!;
    }
}
