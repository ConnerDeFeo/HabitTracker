namespace Server.model.user;

/// <summary>
/// Login result used to dictate authentication
/// </summary>
public class LoginResult {
    public bool Success {get; set;}
    public string SessionKey {get;set;} = string.Empty;
    
}