namespace Server.service.concrete;
using Server.service;
using MongoDB.Driver;
using Server.model.habit;
using Server.model.user;
using MongoDB.Bson;


/// <summary>
/// Concrete implementations of the Habit service class allowing functionality
/// with a mongo database.
/// </summary>
/// <param name="_database">self explanitory</param>
public class MongoHabitService(IMongoDatabase _database) : IHabitService
{

    private readonly IMongoCollection<User> _users = _database.GetCollection<User>("Users");
    private readonly IMongoCollection<HabitCollection> _habitCollections = _database.GetCollection<HabitCollection>("HabitCollection");
    //The followiong are stored filters, updates, and projections that are rather common in the methods for HabitService
    private readonly FilterDefinitionBuilder<User> userFilter = Builders<User>.Filter;
    private readonly FilterDefinitionBuilder<HabitCollection> habitFilter = Builders<HabitCollection>.Filter;
    private readonly UpdateDefinitionBuilder<HabitCollection> update = Builders<HabitCollection>.Update;
    private readonly ProjectionDefinition<HabitCollection> projection = Builders<HabitCollection>.Projection.Include(h => h.Habits);

    public async Task<string?> GetUserIdBySessionKey(string sessionKey)
    {
        User user = await _users.Find(userFilter.Eq(u => u.SessionKey, sessionKey)).FirstOrDefaultAsync();
        return user?.Id;
    }

    public async Task<List<Habit>?> GetHabits(string sessionKey)
    {
        string? userId = await GetUserIdBySessionKey(sessionKey);
        if (userId is not null)
        {
            HabitCollection collection = await _habitCollections
            .Find(habitFilter.Eq(hc => hc.Id, userId))
            .Project<HabitCollection>(projection)
            .FirstOrDefaultAsync();
            return collection.Habits;
        }
        return null;
    }

    public async Task<HabitCollection?> GetHabitCollection(string sessionKey)
    {
        string? userId = await GetUserIdBySessionKey(sessionKey);
        if (userId is not null)
        {
            HabitCollection collection = await _habitCollections
            .Find(habitFilter.Eq(hc => hc.Id, userId))
            .FirstOrDefaultAsync();
            return collection;
        }
        return null;
    }

    /// <summary>
    /// Checks if the given habitid exists in the users current habits
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="habitId"></param>
    /// <returns></returns>
    private async Task<Habit?> HabitIdExists(string userId, string habitId)
    {
        var collection = await _habitCollections
            .Find(hc => hc.Id == userId && hc.Habits.Any(h => h.Id == habitId))
            .FirstOrDefaultAsync();
        return collection?.Habits.FirstOrDefault(h => h.Id == habitId);
    }

    /*For the followiong functions the habit is checked to make sure it exists before
        performing any actions so that the controllers can send back the proper message
        should a habit be deleted, but it never existed, for instance. */

    public async Task<Habit?> CreateHabit(string sessionKey, Habit habit)
    {
        string? userId = await GetUserIdBySessionKey(sessionKey);

        if (userId is not null)
        {
            var existingHabit = await _habitCollections
            .Find(hc => hc.Id == userId && hc.Habits.Any(h => h.Name == habit.Name))
            .FirstOrDefaultAsync();

            if (existingHabit is not null)
                return null;

            habit.Id = ObjectId.GenerateNewId().ToString();
            string today = DateTime.Today.ToString("yyyy-MM-dd");
            var updateHabits = update
                .Push(hc => hc.Habits, habit)
                .Set($"HabitHistory.{today}.{habit.Id}", habit);

            await _habitCollections
            .UpdateOneAsync(
                habitFilter.Eq(hc => hc.Id, userId),
                updateHabits
            );
            return habit;
        }
        return null;
    }

    public async Task<bool> DeleteHabit(string sessionKey, string habitId)
    {
        string? userId = await GetUserIdBySessionKey(sessionKey);
        if (userId is not null)
        {
            Habit? habit = await HabitIdExists(userId, habitId);
            if (habit is null)
                return false;

            var findHabit = habitFilter.And(
                habitFilter.Eq(hc => hc.Id, userId),
                habitFilter.ElemMatch(hc => hc.Habits, h => h.Id == habitId)
            );

            string today = DateTime.Today.ToString("yyyy-MM-dd");
            var combinedUpdate = Builders<HabitCollection>.Update
                .PullFilter(hc => hc.Habits, h => h.Id == habitId)
                .Unset($"HabitHistory.{today}.{habitId}")
                .Push(hc => hc.DeletedHabits, habit!);

            //remove from habits collection
             await _habitCollections
            .UpdateOneAsync(
                findHabit,
                combinedUpdate
            );
            return true;
        }
        return false;
    }

    public async Task<Habit?> EditHabit(string sessionKey, Habit habit)
    {
        string? userId = await GetUserIdBySessionKey(sessionKey);
        if (userId is not null)
        {
            if (habit.Id is null || await HabitIdExists(userId, habit.Id) is null)
                return null;

            var findHabit = habitFilter.And(
                habitFilter.Eq(hc => hc.Id, userId),
                habitFilter.ElemMatch(hc => hc.Habits, h => h.Id == habit.Id)
            );

            string today = DateTime.Today.ToString("yyyy-MM-dd");
            var updateHabits = update
                .Set("Habits.$", habit)
                .Set($"HabitHistory.{today}.{habit.Id}", habit);

            await _habitCollections
            .UpdateOneAsync(
                findHabit,
                updateHabits
            );
            return habit;
        }
        return null;
    }


    public async Task<bool> SetHabitCompletion(string sessionKey, string date, string habitId, bool completed)
    {
        string? userId = await GetUserIdBySessionKey(sessionKey);

        if (userId is not null)
        {
            if (await HabitIdExists(userId, habitId) is null)
                return false;

            date ??= DateTime.Today.ToString("yyyy-MM-dd");
            await _habitCollections.UpdateOneAsync(
                habitFilter.Eq(hc => hc.Id, userId),
                update.Set($"HabitHistory.{date}.{habitId}.Completed", completed)
            );
            return true;
        }
        return false;

    }
    
}