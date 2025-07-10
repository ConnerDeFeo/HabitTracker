using Server.dtos;

namespace Server.service.interfaces;

public interface ISocialDataService
{
    Task<Dictionary<string, string>?> GetRandomUsers(string sessionKey);
    Task<Dictionary<string, string>?> FindUser(string sessionKey, string phrase);
    Task<Dictionary<string, string>?> GetFriends(string sessionKey);
    Task<Profile?> GetProfile(string sessionKey,string username);

}   