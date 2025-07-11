namespace Server.model.habit;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

/// <summary>
/// Represents a single habit
/// </summary>
public class Habit
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public HabitType Type { get; set; } = HabitType.BINARY;

    public string? DateCreated { get; set; }

    public HashSet<string> DaysActive { get; set; } = [];

    public bool Completed { get; set; } = false;

    public bool Skipped { get; set; } = false;

    //The following two only apply to numeric and time habits
    public int Value { get; set; } = 1;

    public string? ValueUnitType { get; set; }

    // override object.Equals
    public override bool Equals(object? obj)
    {
        return obj is Habit other && Name == other.Name;
    }
    
    public override int GetHashCode()
    {
        return Name.GetHashCode();
    }
    
}