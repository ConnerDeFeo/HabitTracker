using MongoDB.Driver;
using Server.model.habit;
using Server.model.user;

namespace Server.service.utils;

public static class BuilderUtils
{
    public static readonly FilterDefinitionBuilder<User> userFilter = Builders<User>.Filter;
    public static readonly FilterDefinitionBuilder<HabitCollection> habitFilter = Builders<HabitCollection>.Filter;
    public static readonly UpdateDefinitionBuilder<HabitCollection> habitUpdate = Builders<HabitCollection>.Update;
    public static readonly UpdateDefinitionBuilder<User> userUpdate = Builders<User>.Update;
    public static readonly FindOneAndUpdateOptions<HabitCollection> habitOptions = new();
    public static readonly FindOneAndUpdateOptions<User> userOptions = new();
    public static readonly ProjectionDefinitionBuilder<HabitCollection> habitProjection = Builders<HabitCollection>.Projection;

}