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

    /// <summary>
    /// Get habits for a specific user for a specific date
    /// </summary>
    /// <param name="sessionKey">user sessionKey</param>
    /// <param name="date">date in yyyt-MM-dd format</param>
    /// <returns>List of habits for a given date for a user</returns>
    public async Task<List<Habit>?> GetHabits(string sessionKey, string date)
    {
        User? user = await UserUtils.GetUserBySessionKey(sessionKey, _users);
        if (user is not null && user.Id is not null)
        {
            string userId = user.Id;
            string month = date[..7];
            string day = date.Substring(8, 2);
            //Only include the given day
            ProjectionDefinition<HabitCollection> habitProjection = BuilderUtils.habitProjection.Include($"HabitHistory.{month}.{day}");
            HabitCollection collection = await _habitCollections
            .Find(
                BuilderUtils.habitFilter.Eq(hc => hc.Id, userId)
            )
            .Project<HabitCollection>(habitProjection)
            .FirstOrDefaultAsync();

            //If date exists, return associated habits
            Dictionary<string, Dictionary<string, HistoricalDate>> history = collection.HabitHistory;
            if (history.TryGetValue(month, out Dictionary<string, HistoricalDate>? value) && value.TryGetValue(day, out HistoricalDate? historicalDate))
                return [.. historicalDate.Habits.Values];
            return null;
        }
        return null;
    }

    /// <summary>
    /// Created habits for a given user, name must not already exist
    /// </summary>
    /// <param name="sessionKey">user sessionKey</param>
    /// <param name="habit">habit being created</param>
    /// <returns>Habit that was created</returns>
    public async Task<Habit?> CreateHabit(string sessionKey, Habit habit)
    {
        User? user = await UserUtils.GetUserBySessionKey(sessionKey, _users);
        if (user is not null && user.Id is not null)
        {
            string userId = user.Id;
            HashSet<Habit> setOfHabits = await HabitUtils.GetAllHabits(userId, _habitCollections);

            /*If there is another habit with a matching name in either
             active or nonactive habits, invalid creation attempt*/
            if (setOfHabits.Contains(habit))
                return null;
            
            //Habit id needs to be mannually made sense it will be held in a list
            habit.Id = ObjectId.GenerateNewId().ToString();
            DateTime today = DateTime.Today;
            habit.DateCreated = today.ToString("yyyy-MM-dd");

            List<UpdateDefinition<HabitCollection>> updates = [];
            updates.Add(
                BuilderUtils.habitUpdate.Push(hc => hc.ActiveHabits, habit)
            );
            //If habit contains todays date, add it to the current days habits and set all habits completed to false
            if (habit.DaysActive.Contains(today.DayOfWeek.ToString()))
                updates.Add(
                    BuilderUtils.habitUpdate
                    .Set($"HabitHistory.{thisMonth}.{thisDay}.Habits.{habit.Id}", habit)
                    .Set($"HabitHistory.{thisMonth}.{thisDay}.AllHabitsCompleted",false)
                );

            BuilderUtils.habitOptions.Projection = Builders<HabitCollection>.Projection.Include($"HabitHistory.{thisMonth}.{thisDay}");
            BuilderUtils.habitOptions.ReturnDocument = ReturnDocument.After;

            HabitCollection collection = await _habitCollections
            .FindOneAndUpdateAsync(
                BuilderUtils.habitFilter.Eq(hc => hc.Id, userId),
                BuilderUtils.habitUpdate.Combine(updates),
                BuilderUtils.habitOptions
            );

            return habit;
        }
        return null;
    }

    /// <summary>
    /// Deactivate a habit 
    /// </summary>
    /// <param name="sessionKey"></param>
    /// <param name="habitId"></param>
    /// <returns>True if habit was deactivated, false otherwise</returns>
    public async Task<bool> DeactivateHabit(string sessionKey, string habitId)
    {
        User? user = await UserUtils.GetUserBySessionKey(sessionKey, _users);
        if (user is not null && user.Id is not null)
        {
            string userId = user.Id;
            HashSet<Habit> setOfHabits = await HabitUtils.GetActiveHabits(userId, _habitCollections);
            Habit? habit = setOfHabits.FirstOrDefault(h => h.Id == habitId);

            //Habit must be in active habits for deactivation
            if (habit is null)
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
               .Push(hc => hc.NonActiveHabits, habit),
               BuilderUtils.habitOptions
           );

            //Deactivation may have lead to completion for the current date
            await HabitUtils.CheckAllHabitsCompleted($"{thisMonth}-{thisDay}", collection, userId, _habitCollections);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Reactivate habit that is currently deactivate
    /// </summary>
    /// <param name="sessionKey">user sessionKey</param>
    /// <param name="habitId">is of habit</param>
    /// <returns>True if habit was reactivated, false else</returns>
    public async Task<bool> ReactivateHabit(string sessionKey, string habitId)
    {
        User? user = await UserUtils.GetUserBySessionKey(sessionKey, _users);
        if (user is not null && user.Id is not null)
        {
            string userId = user.Id;
            HashSet<Habit> setOfHabits = await HabitUtils.GetNonActiveHabits(userId, _habitCollections);
            Habit? habit = setOfHabits.FirstOrDefault(h => h.Id == habitId);
            //Habit must be deactivated to be reactivated
            if (habit is null)
                return false;

            List<UpdateDefinition<HabitCollection>> updates = [];
            updates.Add(
                BuilderUtils.habitUpdate
                .Push(hc => hc.ActiveHabits, habit)
                .PullFilter(hc => hc.NonActiveHabits, h => h.Id == habitId)
            );

            /*If habit has today as a active date, set it for today.
            This also makes todas allhabits completed false*/
            if (habit.DaysActive.Contains(DateTime.Today.DayOfWeek.ToString()))
                updates.Add(
                    BuilderUtils.habitUpdate
                    .Set($"HabitHistory.{thisMonth}.{thisDay}.Habits.{habitId}", habit)
                    .Set($"HabitHistory.{thisMonth}.{thisDay}.AllHabitsCompleted",false)
                );

            BuilderUtils.habitOptions.Projection = Builders<HabitCollection>.Projection.Include($"HabitHistory.{thisMonth}.{thisDay}");
            BuilderUtils.habitOptions.ReturnDocument = ReturnDocument.After;
            //remove from habits collection
            HabitCollection collection = await _habitCollections
           .FindOneAndUpdateAsync(
               BuilderUtils.habitFilter.Eq(hc => hc.Id, userId),
               BuilderUtils.habitUpdate
               .Combine(updates),
               BuilderUtils.habitOptions
           );

            return true;
        }
        return false;
    }

    /// <summary>
    /// Deleted habit from deactivated habits
    /// </summary>
    /// <param name="sessionKey">user sessionKey</param>
    /// <param name="habitId">habit id for deleted hab</param>
    /// <returns>True if habit was delted, false else</returns>
    public async Task<bool> DeleteHabit(string sessionKey, string habitId)
    {
        User? user = await UserUtils.GetUserBySessionKey(sessionKey, _users);
        if (user is not null && user.Id is not null)
        {
            string userId = user.Id;
            HashSet<Habit> setOfHabits = await HabitUtils.GetNonActiveHabits(userId, _habitCollections);
            //Habit must be deactivated for deletion
            if (setOfHabits.FirstOrDefault(h => h.Id == habitId) is null)
                return false;

            var filter = BuilderUtils.habitFilter.Eq(hc => hc.Id, userId);

            BuilderUtils.habitOptions.Projection = Builders<HabitCollection>.Projection.Include($"HabitHistory");
            BuilderUtils.habitOptions.ReturnDocument = ReturnDocument.After;
            //remove from habits collection
            HabitCollection collection = await _habitCollections
           .FindOneAndUpdateAsync(
               filter,
               BuilderUtils.habitUpdate.PullFilter(hc => hc.NonActiveHabits, h => h.Id == habitId),
               BuilderUtils.habitOptions
           );
            List<UpdateDefinition<HabitCollection>> updates = [];

            //Wipe data from history
            foreach (var (month, days) in collection.HabitHistory)
                foreach (var (day, record) in days)
                    if (record.Habits.ContainsKey(habitId))
                    {
                        bool allHabitsCompleted = true;
                        //If the day was not set to all completed, a habit deleted may change that
                        if (!record.AllHabitsCompleted)
                        {
                            foreach (Habit habit in record.Habits.Values)
                                if (!habit.Id!.Equals(habitId) && !habit.Completed)
                                {
                                    allHabitsCompleted = false;
                                    break;
                                }
                        }
                        updates.Add(
                            BuilderUtils.habitUpdate
                            .Unset($"HabitHistory.{month}.{day}.Habits.{habitId}")
                            .Set($"HabitHistory.{month}.{day}.AllHabitsComplted", allHabitsCompleted)
                        );
                    }

            if (updates.Count != 0)
                await _habitCollections.UpdateOneAsync(
                    filter,
                    BuilderUtils.habitUpdate.Combine(updates)
            );
                return true;
        }
        return false;
    }

    /// <summary>
    /// Edits existing habit
    /// </summary>
    /// <param name="sessionKey">user sessionKey</param>
    /// <param name="habit">habit with respective habit Id that is being changed</param>
    /// <returns>Edited Habit</returns>
    public async Task<Habit?> EditHabit(string sessionKey, Habit habit)
    {
        User? user = await UserUtils.GetUserBySessionKey(sessionKey, _users);
        if (user is not null && user.Id is not null)
        {
            string userId = user.Id;
            HashSet<Habit> setOfHabits = await HabitUtils.GetActiveHabits(userId, _habitCollections);

            //Habit must be active in order to edit
            if (habit.Id is null || setOfHabits.FirstOrDefault(h => h.Id!.Equals(habit.Id)) is null)
                return null;

            var filterHabits = BuilderUtils.habitFilter.And(
                BuilderUtils.habitFilter.Eq(hc => hc.Id, userId),
                BuilderUtils.habitFilter.Eq("ActiveHabits.Id", habit.Id)
            );

            List<UpdateDefinition<HabitCollection>> updates = [];
            //Update habit in active habits
            updates.Add(
                BuilderUtils.habitUpdate.Set("ActiveHabits.$", habit)
            );

            //Set or unset habit for today based on the new selected days
            if (habit.DaysActive.Contains(DateTime.Today.DayOfWeek.ToString()))
                updates.Add(
                    BuilderUtils.habitUpdate.Set($"HabitHistory.{thisMonth}.{thisDay}.Habits.{habit.Id}", habit)
                );
            else
                updates.Add(
                     BuilderUtils.habitUpdate.Unset($"HabitHistory.{thisMonth}.{thisDay}.Habits.{habit.Id}")
                 );

            BuilderUtils.habitOptions.Projection = Builders<HabitCollection>.Projection.Include($"HabitHistory");
            BuilderUtils.habitOptions.ReturnDocument = ReturnDocument.After;

            HabitCollection collection = await _habitCollections
            .FindOneAndUpdateAsync(
                filterHabits,
                BuilderUtils.habitUpdate.Combine(updates),
                BuilderUtils.habitOptions
            );

            return habit;
        }
        return null;
    }

    /// <summary>
    /// Gets all existing habits and returns them marked as either active or non active
    /// </summary>
    /// <param name="sessionKey">user sessionKey</param>
    /// <returns>Dict with "ActiveHabits" and "NonActiveHabits" as keys and their repective habits as lists</returns>
    public async Task<Dictionary<string, List<Habit>>?> GetExistingHabits(string sessionKey)
    {
        User? user = await UserUtils.GetUserBySessionKey(sessionKey, _users);
        if (user is not null && user.Id is not null)
        {
            string userId = user.Id;
            ProjectionDefinition<HabitCollection> habitProjection =
            BuilderUtils.habitProjection
            .Include("ActiveHabits")
            .Include("NonActiveHabits");
            HabitCollection collection = await _habitCollections
            .Find(
                BuilderUtils.habitFilter.Eq(hc => hc.Id, userId)
            )
            .Project<HabitCollection>(habitProjection)
            .FirstOrDefaultAsync();

            Dictionary<string, List<Habit>> existingHabits = [];
            existingHabits["ActiveHabits"] = collection.ActiveHabits;
            existingHabits["NonActiveHabits"] = collection.NonActiveHabits;

            return existingHabits;
        }
        return null;
    }

}
