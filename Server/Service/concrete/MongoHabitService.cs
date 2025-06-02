namespace Server.service.concrete;
using Server.service;
using MongoDB.Driver;
using Server.model.habit;
using Server.model.user;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;


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
    private readonly FindOneAndUpdateOptions<HabitCollection> options = new();
    private readonly ProjectionDefinitionBuilder<HabitCollection> projection = Builders<HabitCollection>.Projection;


    private async Task<string?> GetUserIdBySessionKey(string sessionKey)
    {
        User user = await _users.Find(userFilter.Eq(u => u.SessionKey, sessionKey)).FirstOrDefaultAsync();
        return user?.Id;
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

    
    /// <summary>
    /// Checks the current state of some given date to see if all habits for that date were
    /// completed. Then update the AllHabitsCompleted variable in the respective
    /// historical date if needed
    /// </summary>
    /// <param name="date">Date for this collection</param>
    /// <param name="collection">habitcollection, generically should only contain the respective date in its habithistory</param>
    /// <param name="userId">user for which this is occuring</param>
    private  async void CheckAllHabitsCompleted(string date, HabitCollection collection, string userId)
    {
        string thisMonth = date[..7];
        string thisDay = date.Substring(8, 2);
        //If there was a change in all completed habit, set it. 
        HistoricalDate historicalDate = collection.HabitHistory[thisMonth][thisDay];
            bool allCompleted = true;
            foreach (Habit habit in historicalDate.Habits.Values)
                if (!habit.Completed)
                    allCompleted = false;

            if (allCompleted != historicalDate.AllHabitsCompleted)
                await _habitCollections.UpdateOneAsync(
                    habitFilter.Eq(hc => hc.Id, userId),
                    update.Set($"HabitHistory.{thisMonth}.{thisDay}.AllHabitsCompleted", allCompleted)
                );
    }

    public async Task<List<Habit>?> GetHabits(string sessionKey, string date)
    {
        string? userId = await GetUserIdBySessionKey(sessionKey);
        if (userId is not null)
        {
            string thisMonth = date[..7];
            string today = date.Substring(8, 2);

            ProjectionDefinition<HabitCollection> habitProjection = projection.Include($"HabitHistory.{thisMonth}.{today}");
            HabitCollection collection = await _habitCollections
            .Find(
                habitFilter.Eq(hc => hc.Id, userId) 
            )
            .Project<HabitCollection>(habitProjection)
            .FirstOrDefaultAsync();
            Dictionary<string, Dictionary<string, HistoricalDate>> history = collection.HabitHistory;
            if (history.TryGetValue(thisMonth, out Dictionary<string, HistoricalDate>? value) && value.TryGetValue(today, out HistoricalDate? historicalDate)) 
                return [.. historicalDate.Habits.Values];
            return null;
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
            string thisMonth = today[..7];
            string thisDay = today.Substring(8,2);

            var updateHabits = update
                .Push(hc => hc.Habits, habit)
                .Set($"HabitHistory.{thisMonth}.{thisDay}.Habits.{habit.Id}", habit);

            options.Projection = Builders<HabitCollection>.Projection.Include($"HabitHistory.{thisMonth}.{thisDay}");
            options.ReturnDocument = ReturnDocument.After;

            HabitCollection collection = await _habitCollections
            .FindOneAndUpdateAsync(
                habitFilter.Eq(hc => hc.Id, userId),
                updateHabits,
                options
            );

            CheckAllHabitsCompleted(today, collection, userId);

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
            string thisMonth = today[..7];
            string thisDay = today.Substring(8,2);

            var combinedUpdate = Builders<HabitCollection>.Update
                .PullFilter(hc => hc.Habits, h => h.Id == habitId)
                .Unset($"HabitHistory.{thisMonth}.{thisDay}.Habits.{habitId}")
                .Push(hc => hc.DeletedHabits, habit!);

            options.Projection = Builders<HabitCollection>.Projection.Include($"HabitHistory.{thisMonth}.{thisDay}");
            options.ReturnDocument = ReturnDocument.After;
            //remove from habits collection
            HabitCollection collection = await _habitCollections
           .FindOneAndUpdateAsync(
               findHabit,
               combinedUpdate,
               options
           );

            CheckAllHabitsCompleted(today, collection, userId);
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
            string thisMonth = today[..7];
            string thisDay = today.Substring(8,2);

            var updateHabits = update
                .Set("Habits.$", habit)
                .Set($"HabitHistory.{thisMonth}.{thisDay}.Habits.{habit.Id}", habit);

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
            string thisMonth = date[..7];
            string thisDay = date.Substring(8,2);


            options.Projection = projection.Include($"HabitHistory.{thisMonth}.{thisDay}");
            options.ReturnDocument = ReturnDocument.After;
            
            //update and set the new date
            HabitCollection collection = await _habitCollections.FindOneAndUpdateAsync(
                habitFilter.Eq(hc => hc.Id, userId),
                update.Set($"HabitHistory.{thisMonth}.{thisDay}.Habits.{habitId}.Completed", completed),
                options
            );

            //If there was a change in all completed habit, set it. 
            CheckAllHabitsCompleted(date, collection, userId);


            return true;
        }
        return false;
    }

    /// <summary>
    /// Returns the habit hitory of a given month
    /// </summary>
    /// <param name="month">Should be in yyyy-MM format</param>
    /// <returns></returns>
    public async Task<Dictionary<string, HistoricalDate>?> GetHabitHistoryByMonth(string sessionKey, string yearMonth)
    {
        string? userId = await GetUserIdBySessionKey(sessionKey);

        if (userId is not null)
        {
            var filter = habitFilter.And(
                habitFilter.Eq(hc => hc.Id, userId),
                habitFilter.Exists($"HabitHistory.{yearMonth}")
            );
            var proj = projection.Include(hc => hc.HabitHistory);

            HabitCollection collection = await _habitCollections
                .Find(filter)
                .Project<HabitCollection>(proj)
                .FirstOrDefaultAsync();

            if (collection != null && collection.HabitHistory.TryGetValue(yearMonth, out Dictionary<string, HistoricalDate>? month))
                return month;
            return null;
        }

        return null;
    }
}