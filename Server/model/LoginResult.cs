namespace Server.model;

public class LoginResult {
    public bool Success {get; set;}
    public string SessionKey {get;set;} = String.Empty;
    
}