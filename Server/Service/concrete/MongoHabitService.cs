namespace Server.service.concrete;
using Server.service;
using MongoDB.Driver;
using Server.model;
using MongoDB.Bson;


/// <summary>
/// Concrete implementations of the Habit service class allowing functionality
/// with a mongo database.
/// </summary>
/// <param name="_database"></param>
public class MongoHabitService(IMongoDatabase _database) : IHabitService
{

    private readonly IMongoCollection<User> _users = _database.GetCollection<User>("Users");
    private readonly IMongoCollection<HabitCollection> _habitCollections = _database.GetCollection<HabitCollection>("HabitCollection");
    //The followiong are stored filters, updates, and projections that are rather common in the methods for HabitService
    private readonly FilterDefinitionBuilder<User> userFilter = Builders<User>.Filter;
    private readonly FilterDefinitionBuilder<HabitCollection> habitFilter = Builders<HabitCollection>.Filter;
    private readonly UpdateDefinitionBuilder<HabitCollection> update = Builders<HabitCollection>.Update;
    private readonly ProjectionDefinition<HabitCollection> projection = Builders<HabitCollection>.Projection.Include(h => h.Habits);
    private readonly FindOneAndUpdateOptions<HabitCollection> options = new()
    {
        ReturnDocument = ReturnDocument.After
    };

    /// <summary>
    /// Optimized user lookup based on session key given
    /// </summary>
    /// <param name="sessionKey"></param>
    /// <returns></returns>
    public async Task<string?> GetUserIdBySessionKey(string sessionKey)
    {
        User user = await _users.Find(userFilter.Eq(u => u.SessionKey, sessionKey)).FirstOrDefaultAsync();
        return user?.Id;
    }

    public async Task<List<Habit>?> GetHabits(string sessionKey)
    {
        string? id = await GetUserIdBySessionKey(sessionKey);
        if (id != null)
        {
            HabitCollection collection = await _habitCollections
            .Find(habitFilter.Eq(hc => hc.Id, id))
            .Project<HabitCollection>(projection)
            .FirstOrDefaultAsync();
            return collection.Habits;
        }
        return null;
    }

    public async Task<List<Habit>?> CreateHabit(string sessionKey, Habit habit)
    {
        string? id = await GetUserIdBySessionKey(sessionKey);

        if (id != null)
        {
            HabitCollection collection = await _habitCollections
            .FindOneAndUpdateAsync(
                habitFilter.Eq(hc => hc.Id, id),
                update.Push(hc => hc.Habits, habit),
                options
            );
            return collection.Habits;
        }
        return null;
    }

    public async Task<List<Habit>?> DeleteHabit(string sessionKey, Habit habit)
    {
        string? id = await GetUserIdBySessionKey(sessionKey);
        if (id != null)
        {
            var findHabit = habitFilter.And(
                habitFilter.Eq(hc => hc.Id, id),
                habitFilter.ElemMatch(hc => hc.Habits, h => h.Id == habit.Id)
            );

            //remove from habits collection
            HabitCollection collection = await _habitCollections
            .FindOneAndUpdateAsync(
                findHabit,
                update.PullFilter(hc => hc.Habits,h =>h.Id==habit.Id),
                options
            );
            //Add to delted list of habits
            await _habitCollections.FindOneAndUpdateAsync(
                habitFilter.Eq(hc => hc.Id, id),
                update.Push(hc => hc.DeletedHabits, habit)
            );
            return collection?.Habits;
        }
        return null;
    }

    public async Task<List<Habit>?> EditHabit(string sessionKey, Habit habit)
    {
        string? id = await GetUserIdBySessionKey(sessionKey);
        if (id != null)
        {
            var findHabit = habitFilter.And(
                habitFilter.Eq(hc => hc.Id, id),
                habitFilter.ElemMatch(hc => hc.Habits, h => h.Id == habit.Id)
            );

            HabitCollection collection = await _habitCollections
            .FindOneAndUpdateAsync(
                findHabit,
                update.Set("Habits.$.Name", habit.Name),
                options
            );
            return collection?.Habits;
        }
        return null;
    }

}