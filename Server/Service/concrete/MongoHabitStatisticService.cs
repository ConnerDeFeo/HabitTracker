using MongoDB.Driver;
using Server.model.habit;
using Server.model.user;

namespace Server.service.concrete;

public class MongoHabitStatisticService(IMongoDatabase _database) : IHabitStatisticService
{
    private readonly IMongoCollection<User> _users = _database.GetCollection<User>("Users");
    private readonly IMongoCollection<HabitCollection> _habitCollections = _database.GetCollection<HabitCollection>("HabitCollection");
    public async Task<HistoricalData?> GetHistoricalData(string sessionKey, Habit habit)
    {
        User? user = await UserUtils.GetUserBySessionKey(sessionKey, _users);
        if (user != null)
        {
            string userId = user.Id!;
            var filter = BuilderUtils.habitFilter.Eq(hc => hc.Id, user.Id);

            if (!DateTime.TryParse(habit.DateCreated, out DateTime dateCreated))
                throw new Exception("Date could not be parsed properly");
            DateTime today = DateTime.Today;
            DateTime thisMonth = new(today.Year, today.Month, 1);

            //We only want to include the months where the habit could have existed
            List<ProjectionDefinition<HabitCollection>> habitHistoryProjections = [];
            habitHistoryProjections.Add(BuilderUtils.habitProjection.Include($"HabitHistory.{thisMonth:yyyy-MM}"));

            while (dateCreated <= thisMonth) {
                habitHistoryProjections.Add(BuilderUtils.habitProjection.Include($"HabitHistory.{dateCreated:yyyy-MM}"));
                dateCreated = dateCreated.AddMonths(1);
            }
            

            HabitCollection? collection = await _habitCollections
                .Find(filter)
                .Project<HabitCollection>(BuilderUtils.habitProjection.Combine(habitHistoryProjections))
                .FirstOrDefaultAsync();

            (int, int) totalValueAndDays = collection.GetTotalValueCompleted(habit.Id!);
            HistoricalData data = new()
            {
                Habit = habit,
                TotalValueCompleted = totalValueAndDays.Item1,
                DaysCompleted = totalValueAndDays.Item2,
                LongestStreak = collection.GetLongestStreak(habit),
                CurrentStreak = collection.GetCurrentStreak(habit)
            };
        }
        return null;
    }
}