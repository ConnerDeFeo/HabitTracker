using MongoDB.Driver;
using Server.model.habit;
using Server.model.user;

namespace Server.service.concrete;

public class MongoHabitStatisticService(IMongoDatabase _database) : IHabitStatisticService
{
    private readonly IMongoCollection<User> _users = _database.GetCollection<User>("Users");
    private readonly IMongoCollection<HabitCollection> _habitCollections = _database.GetCollection<HabitCollection>("HabitCollection");

    /// <summary>
    /// Get historical data for one habit
    /// </summary>
    /// <param name="sessionKey">user sessionKey</param>
    /// <param name="habitId">habit id that data is being collected for</param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<HistoricalData?> GetHistoricalData(string sessionKey, string habitId)
    {
        User? user = await UserUtils.GetUserBySessionKey(sessionKey, _users);
        if (user != null)
        {
            string userId = user.Id!;


            HashSet<Habit> setOfHabits = await HabitUtils.GetAllHabits(userId, _habitCollections);
            Habit? habit = setOfHabits.FirstOrDefault(h => h.Id == habitId);
            //Habit must exist for this to happen
            if (habit is null)
                return null;

            
            if (!DateTime.TryParse(habit.DateCreated, out DateTime dateCreated))
                throw new Exception("Date could not be parsed properly");
            DateTime today = DateTime.Today;
            DateTime thisMonth = new(today.Year, today.Month, 1);

            var filter = BuilderUtils.habitFilter.Eq(hc => hc.Id, userId);

            //We only want to include the months where the habit could have existed
            List<ProjectionDefinition<HabitCollection>> habitHistoryProjections = [];
            habitHistoryProjections.Add(BuilderUtils.habitProjection.Include($"HabitHistory.{thisMonth:yyyy-MM}"));

            //Add all month sense habit has been created to know to minimize data collection
            while (dateCreated <= thisMonth)
            {
                habitHistoryProjections.Add(BuilderUtils.habitProjection.Include($"HabitHistory.{dateCreated:yyyy-MM}"));
                dateCreated = dateCreated.AddMonths(1);
            }

            HabitCollection? collection = await _habitCollections
                .Find(filter)
                .Project<HabitCollection>(BuilderUtils.habitProjection.Combine(habitHistoryProjections))
                .FirstOrDefaultAsync();

            return new()
            {
                Habit = habit,
                TotalValueCompleted = collection.GetTotalValueCompleted(habit.Id!),
                LongestStreak = collection.GetLongestStreak(habit),
                CurrentStreak = collection.GetCurrentStreak(habit)
            };
        }
        return null;
    }

    /// <summary>
    /// Gets total value for haibts completed during a given month for all month during a given year
    /// </summary>
    /// <param name="sessionKey">user sessionKey</param>
    /// <param name="habitId">habit id this data is being collected for</param>
    /// <param name="year">year this data collection is happening for</param>
    /// <returns>Dict with month names as keys and total value completed for that month as values</returns>
    public async Task<Dictionary<string, int>?> GetTotalValuesByMonth(string sessionKey, string habitId, int year)
    {
        User? user = await UserUtils.GetUserBySessionKey(sessionKey, _users);
        if (user != null)
        {
            string userId = user.Id!;

            HashSet<Habit> setOfHabits = await HabitUtils.GetAllHabits(userId, _habitCollections);
            Habit? habit = setOfHabits.FirstOrDefault(h => h.Id == habitId);

            if (habit is null)
                return null;


            DateTime startDate = new DateTime(year, 1, 1).Date;
            DateTime endDate = new DateTime(year, 12, 1).Date;
            DateTime currentDate = DateTime.Today.Date;

            List<ProjectionDefinition<HabitCollection>> habitHistoryProjections = [];
            //We Only Want to include months that have actually happened or are happening
            while (startDate <= endDate && startDate<=currentDate)
            {
                habitHistoryProjections.Add(BuilderUtils.habitProjection.Include($"HabitHistory.{startDate:yyyy-MM}"));
                startDate = startDate.AddMonths(1);
            }

            HabitCollection collection = await _habitCollections.
            Find(BuilderUtils.habitFilter.Eq(hc => hc.Id, userId))
            .Project<HabitCollection>(BuilderUtils.habitProjection.Include("HabitHistory"))
            .FirstOrDefaultAsync();

            return collection.GetTotalValuesByMonth(habitId, new DateTime(year, 1, 1).Date,endDate);
        }
        return null;
    }
}