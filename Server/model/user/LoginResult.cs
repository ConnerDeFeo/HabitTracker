namespace Server.model.user;

/// <summary>
/// Login result used to dictate authentication
/// </summary>
public class LoginResult
{
    public string SessionKey { get; set; } = string.Empty;

    public UserDto? User { get; set; } 
    
}