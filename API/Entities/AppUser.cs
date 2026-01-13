namespace API.Entities;

public class AppUser
{
    // defined string properties 
    // you have to either mark is as 'required' or assign some initial value to avoid 'Null Reference Problem'. 
    public string Id { get; set; } = Guid.NewGuid().ToString(); // assing some unique id to the object

    public required string DisplayName { get; set; }
    public required string Email { get; set; }

}

// Entity classes typically relate to the tables in the database. each instance of the Entity class represents a row in the database.
// Some people call it Models. We are going to use Object Relational Mapper to map entities to the databse tables. Microsoft provides Entity Framework.
