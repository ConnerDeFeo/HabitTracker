namespace Server.Dtos;

/// <summary>
/// Data transfer object representing the public information 
/// availible about users
/// </summary>
public class UserDto
{
    public string Username { get; set; } = null!;

    public string DateCreated { get; set; } = null!;

    public string? ProfilePhotoKey { get; set; }

    public List<string> Friends { get; set; } = [];
}