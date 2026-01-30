using Microsoft.AspNetCore.Identity;

namespace API.Entities;

public class AppUser : IdentityUser
{
    // defined string properties 
    // you have to either mark is as 'required' or assign some initial value to avoid 'Null Reference Problem'. 

    public required string DisplayName { get; set; }
    public string? ImageUrl { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiry { get; set; }

    // Nav Property related to Member class
    public Member Member { get; set; } = null!; 

}

// Entity classes typically relate to the tables in the database. each instance of the Entity class represents a row in the database.
// Some people call it Models. We are going to use Object Relational Mapper to map entities to the databse tables. Microsoft provides Entity Framework.
