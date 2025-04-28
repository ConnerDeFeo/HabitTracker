namespace Server.model;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class Habit
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    
    public string Name{get;set;} = string.Empty;
}