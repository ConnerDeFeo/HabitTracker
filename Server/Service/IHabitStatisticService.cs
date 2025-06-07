using Server.model.habit;

namespace Server.service;

public interface IHabitStatisticService
{
    Task<HistoricalData?> GetHistoricalData(string sessionKey, Habit habit);
}