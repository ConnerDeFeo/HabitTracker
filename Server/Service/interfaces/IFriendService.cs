using Server.dtos;

namespace Server.service.interfaces;

public interface IFriendService
{
    Task<bool> SendFriendRequest(string sessionKey, string username);
    Task<bool> UnSendFriendRequest(string sessionKey, string username);
    Task<Dictionary<string,string?>?> AcceptFriendRequest(string sessionKey, string username);
    Task<Dictionary<string,string?>?> RemoveFriend(string sessionKey, string username);
    Task<bool> RejectFriend(string sessionKey, string username);
    Task<Dictionary<string,string?>?> GetFriends(string sessionKey);
    Task<ProfileHabits?> GetFriendProfile(string sessionKey,string username);
}   