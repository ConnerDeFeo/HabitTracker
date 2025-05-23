using Server.model.habit;

namespace Server.service;

/// <summary>
/// Interface for the interactions with Habit service classes.
/// </summary>
public interface IHabitService
{
    Task<List<Habit>?> GetHabits(string sessionKey);
    Task<List<Habit>?> CreateHabit(string sessionKey, Habit habit);

    Task<List<Habit>?> DeleteHabit(string sessionKey, Habit habit);

    Task<List<Habit>?> EditHabit(string sessionKey, Habit habit);

    Task<List<Habit>?> SetHabitCompletion(string sessionKey, string date, Habit habit, bool completed);

    Task<HabitCollection?> GetHabitCollection(string sessionKey);
}