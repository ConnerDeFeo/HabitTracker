using Server.model.habit;

namespace Server.service;

public interface IHabitHistoryService
{
    Task<bool> SetHabitCompletion(string sessionKey, string date, string habitId, bool completed);
    Task<Dictionary<string, HistoricalDate>?> GetHabitHistoryByMonth(string sessionKey, string month);
}