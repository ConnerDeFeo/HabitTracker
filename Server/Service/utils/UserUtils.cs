using System.Security.Cryptography;
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
        var Filter = BuilderUtils.userFilter.Exists($"SessionKeys.{sessionKey}");

        return await _users.Find(Filter).FirstOrDefaultAsync();
    }

    //Gets all active habits for the month and their current streak
    private static List<ProfileHabit> GetCurrentHabits(HabitCollection collection)
    {
        List<Habit> activeHabits = collection.ActiveHabits;
        List<ProfileHabit> profile = [];
        foreach (Habit habit in activeHabits)
        {
            profile.Add(
                new ProfileHabit
                {
                    Name = habit.Name,
                    DateCreated = habit.DateCreated!,
                    CurrentStreak = collection.GetCurrentStreak(habit)
                }
            );
        }

        return profile;
    }

    //Returns all days completed for a given month for a given habit without all the internal habits
    private static Dictionary<string, bool> GetDaysCompleted(Dictionary<string, HistoricalDate> dates)
    {
        DateTime today = DateTime.UtcNow.Date;
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
    public static dtos.Profile GetProfile(HabitCollection collection, User user)
    {
        List<ProfileHabit> currentHabits = GetCurrentHabits(collection);

        Dictionary<string, HistoricalDate> dates = collection.HabitHistory[DateTime.UtcNow.ToString("yyyy-MM")];
        Dictionary<string, bool> daysCompleted = GetDaysCompleted(dates);

        return new dtos.Profile
        {
            CurrentHabits = currentHabits,
            CurrentMonthHabitsCompleted = daysCompleted,
            Username = user.Username,
            Id = user.Id!,
            DateCreated = user.DateCreated
        };

    }

    public static string GenerateSessionKey()
    {
        byte[] key = RandomNumberGenerator.GetBytes(32);
        return Convert.ToBase64String(key);
    }

    public async static Task UpdateUserHistory(User user, IMongoCollection<HabitCollection> habitCollection)
    {
        //Get habit collection for updating missing dates
        var filter = Builders<HabitCollection>.Filter.Eq(hc => hc.Id, user.Id);
        HabitCollection collection = await habitCollection
            .Find(filter)
            .Project<HabitCollection>(Builders<HabitCollection>.Projection.Include(hc => hc.ActiveHabits))
            .FirstOrDefaultAsync();
        DateTime today = DateTime.UtcNow.Date;
        List<UpdateDefinition<HabitCollection>> habitHistoryUpdates = HabitUtils.UpdateUserHabitHistory(user, collection, today);
        if (habitHistoryUpdates.Count > 0)
            await habitCollection.UpdateOneAsync(filter, Builders<HabitCollection>.Update.Combine(habitHistoryUpdates));
    }

    //Updates a users session key based on device being logged in from
    public async static Task<string> UpdateSessionKeys(User user, string deviceId, IMongoCollection<User> users)
    {
        string newSessionKey = GenerateSessionKey();
        List<UpdateDefinition<User>> updates = [];
        updates.Add(
            BuilderUtils.userUpdate.Set($"SessionKeys.{newSessionKey}", deviceId).
            Set(u => u.LastLoginDate, DateTime.UtcNow.ToString("yyyy-MM-dd"))
        );

        //If there is an old sessionKey, get rid of it for the device
        string? oldSessionKey = null;
        foreach (var kvp in user.SessionKeys)
            if (kvp.Value == deviceId)
                oldSessionKey = kvp.Key;

        if (oldSessionKey is not null)
            updates.Add(BuilderUtils.userUpdate.Unset($"SessionKeys.{oldSessionKey}"));

        await users.UpdateOneAsync(
            u => u.Username.Equals(user.Username),
            BuilderUtils.userUpdate.Combine(updates)
        );

        return newSessionKey;
    } 
}