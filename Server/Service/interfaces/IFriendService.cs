using Server.model.habit;
using Server.model.user;

namespace Server.service.interfaces;

public interface IFriendService
{
    Task<List<UserDto>> GetFriends(string sessionKey);

    Task<ProfileHabits> GetFriendProfile(string sessionKey);
}   