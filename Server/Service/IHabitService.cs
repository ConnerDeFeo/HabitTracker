using Server.model;

namespace Server.service;

/// <summary>
/// Interface for the interactions with Habit service classes.
/// </summary>
public interface IHabitService{
    Task<List<Habit>?> GetHabits(string sessionKey);
    Task<List<Habit>?> CreateHabit(string sessionKey, Habit habit);

    Task<List<Habit>?> DeleteHabit(string sessionKey, Habit habit);

    Task<List<Habit>?> EditHabit(string sessionKey,Habit habit);

    Task<bool> CompleteHabit(string sessionKey, Habit habit);
}