namespace Server.service.concrete;

using MongoDB.Driver;
using Server.model;
using Server.model.habit;
using Server.model.user;
using Server.service;

public class MongoHabitHistoryService(IMongoDatabase _database) : IHabitHistoryService
{ 
  
    private readonly IMongoCollection<User> _users = _database.GetCollection<User>("Users");
    private readonly IMongoCollection<HabitCollection> _habitCollections = _database.GetCollection<HabitCollection>("HabitCollection");
    private readonly string thisMonth = DateTime.Today.ToString("yyyy-MM");
    private readonly string thisDay = DateTime.Today.ToString("dd");
  
    public async Task<bool> SetHabitCompletion(string sessionKey, string date, string habitId, bool completed)
    {
        string? userId = await GetUserIdBySessionKey(sessionKey);

        if (userId is not null)
        {
            var habitIsReal = await _habitCollections
            .Find(hc => hc.Id == userId && (
                hc.Habits.Any(h => h.Id == habitId)
                ||
                hc.DeletedHabits.Any(h => h.Id == habitId)
            ))
            .FirstOrDefaultAsync();

            if (habitIsReal is null)
                return false;

            date ??= DateTime.Today.ToString("yyyy-MM-dd");
            string thisMonth = date[..7];
            string thisDay = date.Substring(8, 2);


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