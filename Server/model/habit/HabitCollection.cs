namespace Server.model.habit;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using ZstdSharp.Unsafe;

/// <summary>
/// Represents the habits that each user holds,
/// Id should match the corresponding user id.
/// </summary>
public class HabitCollection
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    public List<Habit> ActiveHabits { get; set; } = [];
    public List<Habit> NonActiveHabits { get; set; } = [];
    public Dictionary<string, Dictionary<string, HistoricalDate>> HabitHistory { get; set; } = [];

    ///Gets the total value completed for a given habit
    public int GetTotalValueCompleted(string habitId)
    {
        int total = 0;

        foreach (Dictionary<string, HistoricalDate> monthData in HabitHistory.Values)
            foreach (HistoricalDate HistoricalData in monthData.Values)
                if (HistoricalData.Habits.TryGetValue(habitId, out Habit? h) && h.Completed)
                    total += h.Value;

        return total;

    }

    //Gets the most recent streak for a given habit, today is not required, but it can be added towards the streak
    public int GetCurrentStreak(Habit habit)
    {
        HashSet<string> daysActive = habit.DaysActive;
        DateTime currentDate = DateTime.UtcNow;
        string habitId = habit.Id!;
        int currentStreak = 0;
        
        //today is not required for the streak to exist, but can count towards it
        if (
            daysActive.Contains(currentDate.DayOfWeek.ToString()) &&
            HabitHistory.TryGetValue(currentDate.ToString("yyyy-MM"), out var thisMonth) &&
            thisMonth.TryGetValue(currentDate.ToString("dd"), out var thisDate) &&
            thisDate.Habits.TryGetValue(habitId, out var todaysHabit) &&
            todaysHabit.Completed
        )
            ++currentStreak;

        //Start from yesterday
        currentDate = currentDate.AddDays(-1);

        while (HabitHistory.TryGetValue(currentDate.ToString("yyyy-MM"), out var monthData))
        {
            //If the day isnt part of the habits active days, move on
            if (!daysActive.Contains(currentDate.DayOfWeek.ToString()))
            {
                currentDate = currentDate.AddDays(-1);
                continue;
            }
            /*The habit does exist for the day, so if the day doesnt exist, the habit doesnt exist, or the habit isnt completed,
            this is the end of the current streak*/
            if (
                !monthData.TryGetValue(currentDate.ToString("dd"), out var historicalDate) ||
                !historicalDate.Habits.TryGetValue(habitId, out var h) ||
                !h.Completed
            )
                return currentStreak;
            ++currentStreak;
            currentDate = currentDate.AddDays(-1);
        }

        return currentStreak;
    }

    //Gets longest streak for a given habit
    public int GetLongestStreak(Habit habit)
    {
        DateTime currentDate = DateTime.UtcNow;
        HashSet<string> daysActive = habit.DaysActive;
        string habitId = habit.Id!;
        int highestStreak = 0;
        int currentStreak = 0;
        
        /*Note for this we do check everymonth, even past the habit creation point, but it is assumed we
        are only dealinig with a habit history back to where this habit first created anyway. This can
        be easily changed in the future if need be*/
        while (HabitHistory.TryGetValue(currentDate.ToString("yyyy-MM"), out var monthData))
        {
            //If day doesnt contain the habit, move on
            if (!daysActive.Contains(currentDate.DayOfWeek.ToString()))
            {
                currentDate = currentDate.AddDays(-1);
                continue;
            }
            //Else if the habit exists and is not competed
            if (
                !monthData.TryGetValue(currentDate.ToString("dd"), out var date) ||
                !date.Habits.TryGetValue(habitId, out Habit? h) ||
                !h.Completed
            )
            {
                highestStreak = Math.Max(highestStreak, currentStreak);
                currentStreak = 0;
            }
            //Increment if the habit is completed
            else
                ++currentStreak;
            currentDate = currentDate.AddDays(-1);
        }

        return Math.Max(highestStreak, currentStreak);
    }

    /// <summary>
    /// Gets total value of a habit from start date to 1 year in the future of that date
    /// </summary>
    /// <param name="habitId">id of habit</param>
    /// <param name="startDate">date when data collection begins</param>
    /// <returns></returns>
    public Dictionary<string, int> GetTotalValuesByMonth(string habitId, DateTime startDate)
    {
        Dictionary<string, int> totalValuesByMonth = [];

        for (int i = 0; i < 12; i++)
        {
            string monthKey = startDate.ToString("yyyy-MM");
            string displayMonth = startDate.ToString("MMMM");
            totalValuesByMonth[displayMonth] = 0;

            if (HabitHistory.TryGetValue(monthKey, out var monthData))
                foreach (HistoricalDate date in monthData.Values)
                    if (date.Habits.TryGetValue(habitId, out Habit? habit) && habit.Completed)
                        totalValuesByMonth[displayMonth] += habit.Value;
            startDate = startDate.AddMonths(1);
        }

        return totalValuesByMonth;
    }
}