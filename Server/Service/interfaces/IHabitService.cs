using Server.model.habit;

namespace Server.service.interfaces;

/// <summary>
/// Interface for the interactions with Habit service classes.
/// </summary>
public interface IHabitService
{
    Task<List<Habit>?> GetHabits(string sessionKey, string date);
    Task<Habit?> CreateHabit(string sessionKey, Habit habit);
    Task<bool> DeactivateHabit(string sessionKey, string habitId);
    Task<bool> ReactivateHabit(string sessionKey, string habitId);
    Task<bool> DeleteHabit(string sessionKey, string habitId);
    Task<Habit?> EditHabit(string sessionKey, Habit habit);

    Task<Dictionary<string, List<Habit>>?> GetExistingHabits(string sessionKey);
}