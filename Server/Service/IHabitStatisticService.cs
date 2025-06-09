using Server.model.habit;

namespace Server.service;

public interface IHabitStatisticService
{
    Task<HistoricalData?> GetHistoricalData(string sessionKey, string habitId);

    Task<Dictionary<string, int>?> GetTotalValuesByMonth(string sessionKey, string habitId);
}