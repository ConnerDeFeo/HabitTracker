namespace Server.model;
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
    public List<Habit> Habits { get; set; } = [];
    public List<Habit> DeletedHabits { get; set; } = [];
    public Dictionary<string, List<Habit>> HabitHistory { get; set; } = [];
}