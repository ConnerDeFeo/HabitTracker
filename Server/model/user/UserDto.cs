namespace Server.model.user;

/// <summary>
/// Data transfer object representing the public information 
/// availible about users
/// </summary>
public class UserDto
{
    public string Username { get; set; } = null!;

    public string DateCreated { get; set; } = null!;

}