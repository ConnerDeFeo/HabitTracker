using Amazon;
using MongoDB.Driver;
using Server.dtos;
using Server.model.habit;
using Server.model.user;

namespace Server.service.utils;

public static class UserUtils
{
    public static async Task<User?> GetUserByUsername(string username, IMongoCollection<User> _users)
    {
        var Filter = BuilderUtils.userFilter.Eq(u => u.Username, username);

        return await _users.Find(Filter).FirstOrDefaultAsync();
    }

    public static async Task<User?> GetUserBySessionKey(string sessionKey, IMongoCollection<User> _users)
    {
        var Filter = BuilderUtils.userFilter.Eq(u => u.SessionKey, sessionKey);

        return await _users.Find(Filter).FirstOrDefaultAsync();
    }

    //Gets all active habits for the month and their current streak
    private static List<ProfileHabit> GetCurrentHabits(HabitCollection collection)
    {
        List<Habit> activeHabits = collection.ActiveHabits;
        List<ProfileHabit> profileHabits = [];
        foreach (Habit habit in activeHabits)
        {
            profileHabits.Add(
                new ProfileHabit
                {
                    Name = habit.Name,
                    DateCreated = habit.DateCreated!,
                    CurrentStreak = collection.GetCurrentStreak(habit)
                }
            );
        }

        return profileHabits;
    }

    //Returns all days completed for a given month for a given habit without all the internal habits
    private static Dictionary<string, bool> GetDaysCompleted(Dictionary<string, HistoricalDate> dates)
    {
        DateTime today = DateTime.Today.Date;
        DateTime curr = new(today.Year, today.Month, 1);

        Dictionary<string, bool> daysCompleted = [];

        while (curr <= today)
        {
            string currString = curr.ToString("dd");
            if (dates.TryGetValue(currString, out var historicalDate))
            {
                if (historicalDate.AllHabitsCompleted)
                    daysCompleted[currString] = true;
                else
                    daysCompleted[currString] = false;
            }
            curr = curr.AddDays(1);
        }

        return daysCompleted;
    }

    //Returns a the current profile of a given user for a calender overview and all active habit stats
    public static ProfileHabits GetProfileHabits(HabitCollection collection)
    {
        List<ProfileHabit> currentHabits = GetCurrentHabits(collection);

        Dictionary<string, HistoricalDate> dates = collection.HabitHistory[DateTime.Today.ToString("yyyy-MM")];
        Dictionary<string, bool> daysCompleted = GetDaysCompleted(dates);

        return new ProfileHabits
        {
            CurrentHabits = currentHabits,
            CurrentMonthHabitsCompleted = daysCompleted
        };

    }
}