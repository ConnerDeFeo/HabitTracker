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
    private readonly BuilderUtils builder = new();


    private async Task<string?> GetUserIdBySessionKey(string sessionKey)
    {
        User user = await _users.Find(builder.userFilter.Eq(u => u.SessionKey, sessionKey)).FirstOrDefaultAsync();
        return user?.Id;
    }

    private async Task<HashSet<Habit>> GetAllHabits(string userId)
    {
        HabitCollection collection = await _habitCollections
            .Find(hc => hc.Id == userId)
            .Project<HabitCollection>(
                builder.habitProjection
                .Include("ActiveHabits")
                .Include("NonActiveHabits")
            )
            .FirstOrDefaultAsync();

        HashSet<Habit> habits = [];

        foreach (Habit habit in collection.ActiveHabits)
            habits.Add(habit);

        foreach (Habit habit in collection.NonActiveHabits)
            habits.Add(habit);

        return habits;
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
                    builder.habitFilter.Eq(hc => hc.Id, userId),
                    builder.habitUpdate.Set($"HabitHistory.{thisMonth}.{thisDay}.AllHabitsCompleted", allCompleted)
                );
    }

    public async Task<List<Habit>?> GetHabits(string sessionKey, string date)
    {
        string? userId = await GetUserIdBySessionKey(sessionKey);
        if (userId is not null)
        {
            string thisMonth = date[..7];
            string today = date.Substring(8, 2);

            ProjectionDefinition<HabitCollection> habitProjection = builder.habitProjection.Include($"HabitHistory.{thisMonth}.{today}");
            HabitCollection collection = await _habitCollections
            .Find(
                builder.habitFilter.Eq(hc => hc.Id, userId) 
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

    public async Task<Habit?> CreateHabit(string sessionKey, Habit habit)
    {
        string? userId = await GetUserIdBySessionKey(sessionKey);

        if (userId is not null)
        {
            HashSet<Habit> setOfHabits = await GetAllHabits(userId);

            if (setOfHabits.Contains(habit))
                return null;

            habit.Id = ObjectId.GenerateNewId().ToString();

            DateTime today = DateTime.Today;
            string todayString = today.ToString("yyyy-MM-dd");
            string thisMonth = todayString[..7];
            string thisDay = todayString.Substring(8, 2);

            List<UpdateDefinition<HabitCollection>> updates = [];
            updates.Add(
                builder.habitUpdate.Push(hc => hc.ActiveHabits, habit)
            );
            if (habit.DaysOfTheWeek.Contains(today.DayOfWeek.ToString()))
                updates.Add(
                    builder.habitUpdate
                    .Set($"HabitHistory.{thisMonth}.{thisDay}.Habits.{habit.Id}", habit)
                );

            builder.habitOptions.Projection = Builders<HabitCollection>.Projection.Include($"HabitHistory.{thisMonth}.{thisDay}");
            builder.habitOptions.ReturnDocument = ReturnDocument.After;

            HabitCollection collection = await _habitCollections
            .FindOneAndUpdateAsync(
                builder.habitFilter.Eq(hc => hc.Id, userId),
                builder.habitUpdate.Combine(updates),
                builder.habitOptions
            );

            CheckAllHabitsCompleted(todayString, collection, userId);

            return habit;
        }s
        return null;
    }

    public async Task<bool> DeactivateHabit(string sessionKey, string habitId)
    {
        return false;
    }

    public async Task<bool> RectivateHabit(string sessionKey, string habitId)
    {
        return false;
    }

    public async Task<bool> DeleteHabit(string sessionKey, string habitId)
    {
        string? userId = await GetUserIdBySessionKey(sessionKey);
        if (userId is not null)
        {
            HashSet<Habit> setOfHabits = await GetAllHabits(userId);
            if (setOfHabits.FirstOrDefault(h => h.Id == habitId) is null)
                return false;

            string today = DateTime.Today.ToString("yyyy-MM-dd");
            string thisMonth = today[..7];
            string thisDay = today.Substring(8, 2);
            string[] days = ["Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"];

            List<UpdateDefinition<HabitCollection>> updates = [];
            foreach (string day in days)
            {
                updates.Add(
                    builder.habitUpdate
                    .PullFilter($"NonActiveHabits.{day}", builder.habitFilter.Eq(h => h.Id, habitId))
                );
            }
            updates.Add(
                builder.habitUpdate.Unset($"HabitHistory.{thisMonth}.{thisDay}.Habits.{habitId}")
            );

            builder.habitOptions.Projection = Builders<HabitCollection>.Projection.Include($"HabitHistory.{thisMonth}.{thisDay}");
            builder.habitOptions.ReturnDocument = ReturnDocument.After;
            //remove from habits collection
            HabitCollection collection = await _habitCollections
           .FindOneAndUpdateAsync(
               builder.habitFilter.Eq(hc => hc.Id, userId),
               builder.habitUpdate.Combine(updates),
               builder.habitOptions
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
            HashSet<Habit> setOfHabits = await GetAllHabits(userId);
            if (habit.Id is null || setOfHabits.FirstOrDefault(h=>h.Id==habit.Id) is null)
                return null;

            var findHabit = habitFilter.And(
                builder.habitFilter.Eq(hc => hc.Id, userId),
                habitFilter.ElemMatch(hc => hc.Habits, h => h.Id == habit.Id)
            );

            string today = DateTime.Today.ToString("yyyy-MM-dd");
            string thisMonth = today[..7];
            string thisDay = today.Substring(8,2);

            var updateHabits = builder.habitUpdate
                .Set("Habits.$", habit)
                .Set($"HabitHistory.{thisMonth}.{thisDay}.Habits.{habit.Id}", habit);

            await _habitCollections
            .UpdateOneAsync(
                builder.habitFilter.Eq(hc => hc.Id, userId),
                updateHabits
            );
            return habit;
        }
        return null;
    }
}