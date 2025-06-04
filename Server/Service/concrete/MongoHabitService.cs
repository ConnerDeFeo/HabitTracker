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

    private readonly string thisMonth = DateTime.Today.ToString("yyyy-MM");
    private readonly string thisDay = DateTime.Today.ToString("dd");

    private async Task<HashSet<Habit>> GetAllHabits(string userId)
    {
        HabitCollection collection = await _habitCollections
            .Find(hc => hc.Id == userId)
            .Project<HabitCollection>(
                BuilderUtils.habitProjection
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

    public async Task<List<Habit>?> GetHabits(string sessionKey, string date)
    {
        User? user = await UserUtils.GetUserBySessionKey(sessionKey, _users);
        if (user is not null && user.Id is not null)
        {
            string userId = user.Id;
            string month = date[..7];
            string day = date.Substring(8, 2);
            ProjectionDefinition<HabitCollection> habitProjection = BuilderUtils.habitProjection.Include($"HabitHistory.{month}.{day}");
            HabitCollection collection = await _habitCollections
            .Find(
                BuilderUtils.habitFilter.Eq(hc => hc.Id, userId) 
            )
            .Project<HabitCollection>(habitProjection)
            .FirstOrDefaultAsync();

            Dictionary<string, Dictionary<string, HistoricalDate>> history = collection.HabitHistory;
            if (history.TryGetValue(month, out Dictionary<string, HistoricalDate>? value) && value.TryGetValue(day, out HistoricalDate? historicalDate)) 
                return [.. historicalDate.Habits.Values];
            return null;
        }
        return null;
    }

    public async Task<Habit?> CreateHabit(string sessionKey, Habit habit)
    {
        User? user = await UserUtils.GetUserBySessionKey(sessionKey,_users);
        if (user is not null && user.Id is not null)
        {
            string userId = user.Id;
            HashSet<Habit> setOfHabits = await GetAllHabits(userId);

            if (setOfHabits.Contains(habit))
                return null;

            habit.Id = ObjectId.GenerateNewId().ToString();
            DateTime today = DateTime.Today;

            List<UpdateDefinition<HabitCollection>> updates = [];
            updates.Add(
                BuilderUtils.habitUpdate.Push(hc => hc.ActiveHabits, habit)
            );
            if (habit.DaysActive.Contains(today.DayOfWeek.ToString()))
                updates.Add(
                    BuilderUtils.habitUpdate
                    .Set($"HabitHistory.{thisMonth}.{thisDay}.Habits.{habit.Id}", habit)
                );

            BuilderUtils.habitOptions.Projection = Builders<HabitCollection>.Projection.Include($"HabitHistory.{thisMonth}.{thisDay}");
            BuilderUtils.habitOptions.ReturnDocument = ReturnDocument.After;

            HabitCollection collection = await _habitCollections
            .FindOneAndUpdateAsync(
                BuilderUtils.habitFilter.Eq(hc => hc.Id, userId),
                BuilderUtils.habitUpdate.Combine(updates),
                BuilderUtils.habitOptions
            );

            HabitUtils.CheckAllHabitsCompleted($"{thisMonth}-{thisDay}", collection, userId, _habitCollections);

            return habit;
        }
        return null;
    }

    public async Task<bool> DeactivateHabit(string sessionKey, string habitId)
    {
        User? user = await UserUtils.GetUserBySessionKey(sessionKey,_users);
        if (user is not null && user.Id is not null)
        {
            string userId = user.Id;
            HashSet<Habit> setOfHabits = await GetAllHabits(userId);
            Habit? habit = setOfHabits.FirstOrDefault(h => h.Id == habitId);
            if(habit is null)
                return false;

            BuilderUtils.habitOptions.Projection = Builders<HabitCollection>.Projection.Include($"HabitHistory.{thisMonth}.{thisDay}");
            BuilderUtils.habitOptions.ReturnDocument = ReturnDocument.After;
            //remove from habits collection
            HabitCollection collection = await _habitCollections
           .FindOneAndUpdateAsync(
               BuilderUtils.habitFilter.Eq(hc => hc.Id, userId),
               BuilderUtils.habitUpdate
               .PullFilter(hc => hc.ActiveHabits, h => h.Id == habitId)
               .Unset($"HabitHistory.{thisMonth}.{thisDay}.Habits.{habitId}")
               .Push(hc=>hc.NonActiveHabits, habit),
               BuilderUtils.habitOptions
           );

            HabitUtils.CheckAllHabitsCompleted($"{thisMonth}-{thisDay}", collection, userId, _habitCollections);
            return true;
        }
        return false;
    }

    public async Task<bool> RectivateHabit(string sessionKey, string habitId)
    {
        User? user = await UserUtils.GetUserBySessionKey(sessionKey,_users);
        if (user is not null && user.Id is not null)
        {
            string userId = user.Id;
            HashSet<Habit> setOfHabits = await GetAllHabits(userId);
            Habit? habit = setOfHabits.FirstOrDefault(h => h.Id == habitId);
            if(habit is null)
                return false;

            BuilderUtils.habitOptions.Projection = Builders<HabitCollection>.Projection.Include($"HabitHistory.{thisMonth}.{thisDay}");
            BuilderUtils.habitOptions.ReturnDocument = ReturnDocument.After;
            //remove from habits collection
            HabitCollection collection = await _habitCollections
           .FindOneAndUpdateAsync(
               BuilderUtils.habitFilter.Eq(hc => hc.Id, userId),
               BuilderUtils.habitUpdate
               .PullFilter(hc => hc.NonActiveHabits, h => h.Id == habitId)
               .Set($"HabitHistory.{thisMonth}.{thisDay}.Habits.{habitId}", habit)
               .Push(hc=>hc.ActiveHabits, habit),
               BuilderUtils.habitOptions
           );

            HabitUtils.CheckAllHabitsCompleted($"{thisMonth}-{thisDay}", collection, userId, _habitCollections);
            return true;
        }
        return false;
    }

    public async Task<bool> DeleteHabit(string sessionKey, string habitId)
    {
        User? user = await UserUtils.GetUserBySessionKey(sessionKey,_users);
        if (user is not null && user.Id is not null)
        {
            string userId = user.Id;
            HashSet<Habit> setOfHabits = await GetAllHabits(userId);
            if(setOfHabits.FirstOrDefault(h => h.Id == habitId) is null)
                return false;

            BuilderUtils.habitOptions.Projection = Builders<HabitCollection>.Projection.Include($"HabitHistory.{thisMonth}.{thisDay}");
            BuilderUtils.habitOptions.ReturnDocument = ReturnDocument.After;
            //remove from habits collection
            HabitCollection collection = await _habitCollections
           .FindOneAndUpdateAsync(
               BuilderUtils.habitFilter.Eq(hc => hc.Id, userId),
               BuilderUtils.habitUpdate.PullFilter(hc => hc.NonActiveHabits, h => h.Id == habitId),
               BuilderUtils.habitOptions
           );

            HabitUtils.CheckAllHabitsCompleted($"{thisMonth}-{thisDay}", collection, userId, _habitCollections);
            return true;
        }
        return false;
    }

    public async Task<Habit?> EditHabit(string sessionKey, Habit habit)
    {
        User? user = await UserUtils.GetUserBySessionKey(sessionKey,_users);
        if (user is not null && user.Id is not null)
        {
            string userId = user.Id;
            HashSet<Habit> setOfHabits = await GetAllHabits(userId);
            if (habit.Id is null || setOfHabits.FirstOrDefault(h=>h.Id!.Equals(habit.Id)) is null)
                return null;

            var filterHabits = BuilderUtils.habitFilter.And(
                BuilderUtils.habitFilter.Eq(hc => hc.Id, userId),
                BuilderUtils.habitFilter.Eq("ActiveHabits.Id", habit.Id)
            );

            var updateHabits = BuilderUtils.habitUpdate
                .Set("ActiveHabits.$", habit)
                .Set($"HabitHistory.{thisMonth}.{thisDay}.Habits.{habit.Id}", habit);

            await _habitCollections
            .UpdateOneAsync(
                filterHabits,
                updateHabits
            );
            return habit;
        }
        return null;
    }
}