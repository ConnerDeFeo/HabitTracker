namespace Server.model.habit;

public class HistoricalDate
{
    public Dictionary<string, Habit> Habits { get; set; } = [];
    public bool AllHabitsCompleted { get; set; } = false;

    public string DateLookUpKey { get; set; } = null!;

}