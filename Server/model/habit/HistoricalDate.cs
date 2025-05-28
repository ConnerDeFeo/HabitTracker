
using MongoDB.Bson.Serialization.Attributes;

namespace Server.model.habit;

public class HistoricalDate
{
    public Dictionary<string, Habit> Habits { get; set; } = [];
    public bool AllHabitsCompleted { get; set; } = false;

}