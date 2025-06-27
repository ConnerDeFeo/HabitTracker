using MongoDB.Driver;
using Server.model.habit;

namespace Server.service;

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
}