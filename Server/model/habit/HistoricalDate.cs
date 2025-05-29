namespace Server.model.habit;
using MongoDB.Bson.Serialization.Attributes;

[BsonIgnoreExtraElements]
public class HistoricalDate
{
    public Dictionary<string, Habit> Habits { get; set; } = [];
    public bool AllHabitsCompleted { get; set; } = true;

}