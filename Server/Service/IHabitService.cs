using Server.model;

namespace Server.service;

/// <summary>
/// Interface for the interactions with Habit service classes.
/// </summary>
public interface IHabitService{
    Task<List<Habit>?> GetHabits(string sessionKey);
    Task<List<Habit>?> CreateHabit(string sessionKey, string habitName);
}