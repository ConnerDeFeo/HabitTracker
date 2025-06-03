namespace Server.model.habit;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

/// <summary>
/// Represents the habits that each user holds,
/// Id should match the corresponding user id.
/// </summary>
public class HabitCollection
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    public List<Habit> ActiveHabits { get; set; } = [];
    public List<Habit> NonActiveHabits { get; set; } = [];
    public Dictionary<string, Dictionary<string, HistoricalDate>> HabitHistory { get; set; } = [];
}