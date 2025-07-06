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

    public string Username { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public string SessionKey { get; set; } = string.Empty;

    public string LastLoginDate { get; set; } = DateTime.Today.ToString("yyyy-MM-dd");

    public string DateCreated { get; set; } = string.Empty;

    public string? ProfilePhotoKey { get; set; }

    //Map userid : photourl
    public Dictionary<string, string?> Friends { get; set; } = [];
    public Dictionary<string, string?> FriendRequests { get; set; } = [];
    public List<string> FriendRequestsSent { get; set; } = [];

}