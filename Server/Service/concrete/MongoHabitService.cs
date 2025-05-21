namespace Server.service.concrete;
using Server.service;
using MongoDB.Driver;
using Server.model;
using MongoDB.Bson;
using System.Collections.ObjectModel;


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
        ReturnDocument = ReturnDocument.After,
    };


    private void SetArrayFilters(Habit habit)
    {
        options.ArrayFilters =[new JsonArrayFilterDefinition<Habit>("{ 'h.Id': '" + habit.Id + "' }")];
    }
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
        if (id is not null)
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

        if (id is not null)
        {
            string today = DateTime.Today.ToString("yyyy-MM-dd");
            var updateHabits = update.Combine(
                update.Push(hc=>hc.Habits,habit),
                update.Push($"HabitHistory.{today}", habit)
            );

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
        if (id is not null)
        {
            var findHabit = habitFilter.And(
                habitFilter.Eq(hc => hc.Id, id),
                habitFilter.ElemMatch(hc => hc.Habits, h => h.Id == habit.Id)
            );

            string today = DateTime.Today.ToString("yyyy-MM-dd");
            var combinedUpdate = Builders<HabitCollection>.Update
                .PullFilter(hc => hc.Habits, h => h.Id == habit.Id)
                .PullFilter(hc => hc.HabitHistory[today], h => h.Id == habit.Id)
                .Push(hc => hc.DeletedHabits, habit);

            //remove from habits collection
            HabitCollection collection = await _habitCollections
            .FindOneAndUpdateAsync(
                findHabit,
                combinedUpdate,
                options
            );
            return collection?.Habits;
        }
        return null;
    }

    public async Task<List<Habit>?> EditHabit(string sessionKey, Habit habit)
    {
        string? id = await GetUserIdBySessionKey(sessionKey);
        if (id is not null)
        {
            var findHabit = habitFilter.And(
                habitFilter.Eq(hc => hc.Id, id),
                habitFilter.ElemMatch(hc => hc.Habits, h => h.Id == habit.Id)
            );

            string today = DateTime.Today.ToString("yyyy-MM-dd");
            SetArrayFilters(habit);

            var updateHabits = update
                .Set("Habits.$[h]", habit)
                .Set($"HabitHistory.{today}.$[h]", habit);

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

    public async Task<List<Habit>?> CompleteHabit(string sessionKey, Habit habit, string date)
    {
        string? id = await GetUserIdBySessionKey(sessionKey);

        if (id is not null)
        {
            //Set array filters so that the only element in the array updated is the one with the matching Id
            SetArrayFilters(habit);

            HabitCollection updatedCollection = await _habitCollections.FindOneAndUpdateAsync(
                habitFilter.Eq(hc => hc.Id, id),
                update.Set($"HabitHistory.{date}.$[h].Completed",true),
                options
            );
        }
        return null;

    }
}