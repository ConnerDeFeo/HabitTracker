using Server.dtos;

namespace Server.service.interfaces;

public interface IFriendService
{
    Task<Dictionary<string, string>?> GetRandomUsers(string sessionKey);
    Task<Dictionary<string, string>?> FindUser(string sessionKey, string phrase);
    Task<Dictionary<string, string>?> GetFriends(string sessionKey);
    Task<Profile?> GetFriendProfile(string sessionKey,string friendUsername);
    Task<bool> SendFriendRequest(string sessionKey, string friendUsername);
    Task<bool> UnSendFriendRequest(string sessionKey, string friendUsername);
    Task<Dictionary<string,string>?> AcceptFriendRequest(string sessionKey, string friendUsername);
    Task<Dictionary<string,string>?> RemoveFriend(string sessionKey, string friendUsername);
    Task<bool> RejectFriendRequest(string sessionKey, string friendUsername);
}   