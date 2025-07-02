using Server.model.habit;

namespace Server.Dtos;

public class HistoricalData
{
    public Habit Habit { get; set; } = null!;
    public int TotalValueCompleted { get; set; } = 0;
    public int DaysCompleted { get; set; } = 0;
    public int LongestStreak { get; set; } = 0;
    public int CurrentStreak { get; set; } = 0;

}
