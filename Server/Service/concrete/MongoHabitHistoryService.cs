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
        User? user = await UserUtils.GetUserBySessionKey(sessionKey,_users);
        if (user is not null && user.Id is not null)
        {
            string userId = user.Id;
            HashSet<Habit> setOfHabits = await HabitUtils.GetAllHabits(userId, _habitCollections);

            Habit? habit = setOfHabits.FirstOrDefault(h => h.Id == habitId);
            if(habit is null)
                return false;

            if (!DateTime.TryParse(date, out DateTime convertedDate) || !habit.DaysActive.Contains(convertedDate.DayOfWeek.ToString()))
                return false;
            string month = date[..7];
            string day = date.Substring(8, 2);


            BuilderUtils.habitOptions.Projection = BuilderUtils.habitProjection.Include($"HabitHistory.{thisMonth}.{thisDay}");
            BuilderUtils.habitOptions.ReturnDocument = ReturnDocument.After;

            //update and set the new date
            HabitCollection collection = await _habitCollections.FindOneAndUpdateAsync(
                BuilderUtils.habitFilter.Eq(hc => hc.Id, userId),
                BuilderUtils.habitUpdate.Set($"HabitHistory.{month}.{day}.Habits.{habitId}.Completed", completed),
                BuilderUtils.habitOptions
            );

            //If there was a change in all completed habit, set it. 
            HabitUtils.CheckAllHabitsCompleted($"{month}-{day}", collection, userId, _habitCollections);


            if (!collection.HabitHistory.TryGetValue(month, out var monthDict) || !monthDict.TryGetValue(day, out var dayDict))
                return false;

            return dayDict.Habits.ContainsKey(habitId);
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
        User? user = await UserUtils.GetUserBySessionKey(sessionKey,_users);
        if (user is not null && user.Id is not null)
        {
            string userId = user.Id;
            var filter = BuilderUtils.habitFilter.And(
                BuilderUtils.habitFilter.Eq(hc => hc.Id, userId),
                BuilderUtils.habitFilter.Exists($"HabitHistory.{yearMonth}")
            );
            var proj = BuilderUtils.habitProjection.Include(hc => hc.HabitHistory);

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