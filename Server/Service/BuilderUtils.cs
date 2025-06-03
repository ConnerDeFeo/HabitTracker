using MongoDB.Driver;
using Server.model.habit;
using Server.model.user;

namespace Server.service;

public class BuilderUtils
{
    public readonly FilterDefinitionBuilder<User> userFilter = Builders<User>.Filter;
    public readonly FilterDefinitionBuilder<HabitCollection> habitFilter = Builders<HabitCollection>.Filter;
    public readonly UpdateDefinitionBuilder<HabitCollection> habitUpdate = Builders<HabitCollection>.Update;
    public readonly UpdateDefinitionBuilder<User> userUpdate = Builders<User>.Update;
    public readonly FindOneAndUpdateOptions<HabitCollection> options = new();
    public readonly ProjectionDefinitionBuilder<HabitCollection> projection = Builders<HabitCollection>.Projection;
}