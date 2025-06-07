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

    public (int, int) GetTotalValueCompleted(string habitId)
    {
        int total = 0;
        int daysCompleted = 0;

        foreach (var (month, monthData) in HabitHistory)
            foreach (var (day, HistoricalData) in monthData)
                if (HistoricalData.Habits.TryGetValue(habitId, out Habit? h) && h.Completed)
                {
                    total += h.Value;
                    ++daysCompleted;
                }

        return (total, daysCompleted);

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
                continue;
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

        return currentStreak;
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
                continue;
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
        }
            
        return Math.Max(highestStreak,currentStreak);
    }
}