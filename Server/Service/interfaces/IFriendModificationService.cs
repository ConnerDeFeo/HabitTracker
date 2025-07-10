namespace Server.service.interfaces;

public interface IFriendModificationService
{
    Task<bool> SendFriendRequest(string sessionKey, string friendUsername);
    Task<bool> UnSendFriendRequest(string sessionKey, string friendUsername);
    Task<Dictionary<string,string>?> AcceptFriendRequest(string sessionKey, string friendUsername);
    Task<Dictionary<string,string>?> RemoveFriend(string sessionKey, string friendUsername);
    Task<bool> RejectFriendRequest(string sessionKey, string friendUsername);
}   