namespace Server.model;

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
    
    public string Name{get;set;} = string.Empty;

    public HabitType? Type{get;set;}

    public bool IsActive{get;set;} = true;

    public override bool Equals(object? obj){
        if(obj!=null && obj is Habit other){
            return other.Id==Id;
        }
        return false;
    }

    public override int GetHashCode(){
        return base.GetHashCode();
    }
}