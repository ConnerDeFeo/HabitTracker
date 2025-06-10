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

    public int GetTotalValueCompleted(string habitId)
    {
        int total = 0;

        foreach (Dictionary<string,HistoricalDate> monthData in HabitHistory.Values)
            foreach (HistoricalDate HistoricalData in monthData.Values)
                if (HistoricalData.Habits.TryGetValue(habitId, out Habit? h) && h.Completed)
                    total += h.Value;

        return total;

    }

    public int GetCurrentStreak(Habit habit)
    {
        HashSet<string> daysActive = habit.DaysActive;
        DateTime currentDate = DateTime.Today.AddDays(-1);
        string habitId = habit.Id!;
        int currentStreak = 0;

        while (HabitHistory.TryGetValue(currentDate.ToString("yyyy-MM"), out var monthData))
        {
            if (!daysActive.Contains(currentDate.DayOfWeek.ToString()))
            {
                currentDate = currentDate.AddDays(-1);
                continue;
            }
            if
            (
                !monthData.TryGetValue(currentDate.ToString("dd"), out var historicalDate) ||
                !historicalDate.Habits.TryGetValue(habitId, out var h) ||
                !h.Completed
            )
                break;
            ++currentStreak;
            currentDate = currentDate.AddDays(-1);
        }

        DateTime today = DateTime.Today;

        if (
            !daysActive.Contains(today.DayOfWeek.ToString()) ||
            !HabitHistory.TryGetValue(today.ToString("yyyy-MM"), out var thisMonth) ||
            !thisMonth.TryGetValue(today.ToString("dd"), out var thisDate) ||
            !thisDate.Habits.TryGetValue(habitId, out var todaysHabit) ||
            !todaysHabit.Completed
        )
            return currentStreak;
        return currentStreak + 1;
    }

    public int GetLongestStreak(Habit habit)
    {
        DateTime currentDate = DateTime.Today;
        HashSet<string> daysActive = habit.DaysActive;
        string habitId = habit.Id!;
        int highestStreak = 0;
        int currentStreak = 0;

        while (HabitHistory.TryGetValue(currentDate.ToString("yyyy-MM"), out var monthData))
        {
            if (!daysActive.Contains(currentDate.DayOfWeek.ToString()))
            {
                currentDate = currentDate.AddDays(-1);
                continue;
            }
            if
            (
                !monthData.TryGetValue(currentDate.ToString("dd"), out var date) ||
                !date.Habits.TryGetValue(habitId, out Habit? h) ||
                !h.Completed
            )
            {
                highestStreak = Math.Max(highestStreak, currentStreak);
                currentStreak = 0;
            }
            else
                ++currentStreak;
            currentDate = currentDate.AddDays(-1);
        }

        return Math.Max(highestStreak, currentStreak);
    }

    public Dictionary<string, int> GetTotalValuesByMonth(string habitId, int? yearBackwards)
    {
        int yearsBack = yearBackwards ??  0;
        Dictionary<string, int> totalValuesByMonth = [];
        DateTime currentMonth = DateTime.Today.AddYears(-Math.Abs(yearsBack));

        while (HabitHistory.TryGetValue(currentMonth.ToString("yyyy-MM"), out var monthData))
        {
            string month = currentMonth.ToString("MMMM");
            if (totalValuesByMonth.ContainsKey(month))
                break;

            totalValuesByMonth[month] = 0;
            foreach (HistoricalDate date in monthData.Values)
                if (date.Habits.TryGetValue(habitId, out Habit? habit) && habit.Completed)
                    totalValuesByMonth[month] += habit.Value;
        }

        return totalValuesByMonth;
    }
}