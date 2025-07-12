using MongoDB.Driver;
using Server.model.habit;
using Server.model.user;

namespace Server.service.utils;

public static class HabitUtils
{

    /// <summary>
    /// Checks the current state of some given date to see if all habits for that date were
    /// completed. Then update the AllHabitsCompleted variable in the respective
    /// historical date if needed
    /// </summary>
    /// <param name="date">Date for this collection</param>
    /// <param name="collection">habitcollection, generically should only contain the respective date in its habithistory</param>
    /// <param name="userId">user for which this is occuring</param>
    public static async Task CheckAllHabitsCompleted(string date, HabitCollection collection, string userId, IMongoCollection<HabitCollection> _habitCollections)
    {
        string month = date[..7];
        string day = date.Substring(8, 2);

        HistoricalDate historicalDate = collection.HabitHistory[month][day];
        bool allCompleted = true;
        foreach (Habit habit in historicalDate.Habits.Values)
            if (!habit.Completed)
            {
                allCompleted = false;
                break;
            }

        if (allCompleted != historicalDate.AllHabitsCompleted)
            await _habitCollections.UpdateOneAsync(
                BuilderUtils.habitFilter.Eq(hc => hc.Id, userId),
                BuilderUtils.habitUpdate.Set($"HabitHistory.{month}.{day}.AllHabitsCompleted", allCompleted)
            );
    }

    public static async Task<HashSet<Habit>> GetAllHabits(string userId, IMongoCollection<HabitCollection> habitCollections)
    {
        HabitCollection collection = await habitCollections
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

    public static async Task<HashSet<Habit>> GetNonActiveHabits(string userId, IMongoCollection<HabitCollection> habitCollections)
    {
        HabitCollection collection = await habitCollections
            .Find(hc => hc.Id == userId)
            .Project<HabitCollection>(
                BuilderUtils.habitProjection
                .Include("NonActiveHabits")
            )
            .FirstOrDefaultAsync();

        HashSet<Habit> nonActiveHabits = [];

        foreach (Habit habit in collection.NonActiveHabits)
            nonActiveHabits.Add(habit);

        return nonActiveHabits;
    }

    public static async Task<HashSet<Habit>> GetActiveHabits(string userId, IMongoCollection<HabitCollection> habitCollections)
    {
        HabitCollection collection = await habitCollections
            .Find(hc => hc.Id == userId)
            .Project<HabitCollection>(
                BuilderUtils.habitProjection
                .Include("ActiveHabits")
            )
            .FirstOrDefaultAsync();

        HashSet<Habit> activeHabits = [];

        foreach (Habit habit in collection.ActiveHabits)
            activeHabits.Add(habit);

        return activeHabits;
    }

    //Parses last login date for any given habit
    private static DateTime ParseLastLoginDate(User user)
    {
        if (!DateTime.TryParse(user.LastLoginDate, out DateTime lastLogin))
            throw new Exception("Date not parsed properly");
        lastLogin = lastLogin.Date;
        return lastLogin;
    } 
    

    /// <summary>
    /// Returns list of updates needed for a given user loggin in
    /// </summary>
    /// <param name="user">user being updated</param>
    /// <param name="collection">collection of given user</param>
    /// <returns>List of update definitions for a service to call on</returns>
    /// <exception cref="Exception">Date could not be parsed properly</exception>
    public static List<UpdateDefinition<HabitCollection>> UpdateUserHabitHistory(User user, HabitCollection collection, DateTime toDate)
    {
        List<UpdateDefinition<HabitCollection>> habitHistoryUpdates = [];
        UpdateDefinitionBuilder<HabitCollection> updateHabitCollection = Builders<HabitCollection>.Update;
        DateTime lastLogin = ParseLastLoginDate(user);

        if (lastLogin != toDate)
        {
            Dictionary<DayOfWeek, HistoricalDate> daysToHabits = [];
            /*For every day there has not been a login and today, set the habit history as the blank slate
            of incomplete haibts*/
            while (lastLogin <= toDate)
            {
                DayOfWeek dayOfWeek = lastLogin.DayOfWeek;
                //we only need to do this once for each day of the week
                if (!daysToHabits.ContainsKey(dayOfWeek))
                {
                    daysToHabits[dayOfWeek] = new();

                    foreach (Habit habit in collection.ActiveHabits)
                        if (habit.DaysActive.Contains(lastLogin.DayOfWeek.ToString()))
                        {
                            daysToHabits[lastLogin.DayOfWeek].Habits[habit.Id!] = habit;
                            daysToHabits[dayOfWeek].AllHabitsCompleted = false;
                        }
                }

                //add the dict to the db
                habitHistoryUpdates.Add(
                    updateHabitCollection.Set($"HabitHistory.{lastLogin:yyyy-MM}.{lastLogin:dd}", daysToHabits[lastLogin.DayOfWeek])
                );

                lastLogin = lastLogin.AddDays(1);
            }
            //complete all updates on the dictionary
        }
        return habitHistoryUpdates;
    }
}