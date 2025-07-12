namespace Server.model.user;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

/// <summary>
/// Main user class representing the user objects.
/// This is the highest level of data availible with
/// all other forms nested in this class in some way.
/// </summary>
public class User
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public string Email { get; set; } = null!;

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public Dictionary<string,string> SessionKeys { get; set; } = [];

    public string LastLoginDate { get; set; } = DateTime.UtcNow.ToString("yyyy-MM-dd");

    public string DateCreated { get; set; } = string.Empty;

    //Map userid : photourl
    public Dictionary<string, string> Friends { get; set; } = [];
    public Dictionary<string, string> FriendRequests { get; set; } = [];
    public List<string> FriendRequestsSent { get; set; } = [];

}