using Server.model.habit;

namespace Server.service.interfaces;

public interface IFriendService
{
    Task<List<ProfileHabit>?> GetProfileHabits(string sessionKey, string? username);
}