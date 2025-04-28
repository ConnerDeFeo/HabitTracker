using Server.model;

namespace Server.service;

public interface IHabitService{
    Task<List<Habit>?> GetHabits(string sessionKey);
    Task<Habit?> CreateHabit(string sessionKey, string habitName);
}