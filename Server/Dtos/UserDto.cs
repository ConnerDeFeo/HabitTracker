namespace Server.dtos;

/// <summary>
/// Data transfer object representing the public information 
/// availible about users
/// </summary>
public class UserDto
{
    public string Username { get; set; } = null!;

    public string DateCreated { get; set; } = null!;

    public string UserId { get; set; } = null!;

    public Dictionary<string,string> Friends { get; set; } = [];
    public Dictionary<string,string> FriendRequests { get; set; } = [];
    public List<string> FriendRequestsSent { get; set; } = [];
}