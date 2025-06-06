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
                if (HistoricalData.Habits.TryGetValue(habitId, out Habit? h))
                {
                    total += h.Value;
                    ++daysCompleted;
                }

        return (total, daysCompleted);

    }

    public int GetCurrentStreak(Habit habit, DateTime? dateCreated)
    {
        DateTime today = DateTime.Today;
        if (dateCreated is null && DateTime.TryParse(habit.DateCreated, out var parsed))
            dateCreated = parsed;

        List<string> monthsInOrder = [.. HabitHistory.Keys];
        monthsInOrder.Sort();

        string habitId = habit.Id!;
        int currentStreak = 0;

        while (HabitHistory.TryGetValue(today.ToString("yyyy-MM"), out var monthData))
        {
            while (monthData.TryGetValue(today.ToString("dd"), out var date))
            {
                if (date.Habits.ContainsKey(habitId))
                {
                    if (date.Habits[habitId].Completed)
                    {
                        ++currentStreak;
                        _ = today.AddDays(-1);
                        continue;
                    }
                }
                return currentStreak;

            }
        }
        return currentStreak;
        
        
    }
    
    // public int GetLongestStreak(Habit habit, DateTime? dateCreated)
    // { 
    //     if (dateCreated is null && DateTime.TryParse(habit.DateCreated, out var parsed))
    //         dateCreated = parsed;
    // }
}