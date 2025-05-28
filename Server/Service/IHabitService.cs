using Server.model.habit;

namespace Server.service;

/// <summary>
/// Interface for the interactions with Habit service classes.
/// </summary>
public interface IHabitService
{
    Task<List<Habit>?> GetHabits(string sessionKey);
    Task<Habit?> CreateHabit(string sessionKey, Habit habit);

    Task<bool> DeleteHabit(string sessionKey, string habitId);

    Task<Habit?> EditHabit(string sessionKey, Habit habit);

    Task<bool> SetHabitCompletion(string sessionKey, string date, string habitId, bool completed);

    Task<HabitCollection?> GetHabitCollection(string sessionKey);

    Task<Dictionary<string, HistoricalDate>?> GetHabitHistoryByMonth(string sessionKey, string month);
}