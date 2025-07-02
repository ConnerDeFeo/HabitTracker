using Server.dtos;

namespace Server.service.interfaces;

public interface IFriendService
{
    Task<List<UserDto>> GetFriends(string sessionKey);

    Task<ProfileHabits> GetFriendProfile(string sessionKey);
}   