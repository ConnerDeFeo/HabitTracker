namespace Test.service;
using MongoDB.Driver;
using Server.service;
using Server.service.concrete;
using Server.model.user;
using Server.model.habit;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using MongoDB.Bson;

public class TestMongoHabitStatistic
{
    string monthKey;
    string dayKey;
    IMongoDatabase database;
    IUserService userService;
    IHabitService habitService;
    IHabitHistoryService habitHistoryService;
    IHabitStatisticService habitStatisticService;
    HashSet<string> daysOfWeek;

    public TestMongoHabitStatistic()
    {
        var Client = new MongoClient("mongodb://localhost:27017");
        Client.DropDatabase("TestMongoStatisticService");
        database = Client.GetDatabase("TestMongoStatisticService");
        userService = new MongoUserService(database);
        habitService = new MongoHabitService(database);
        habitHistoryService = new MongoHabitHistoryService(database);
        habitStatisticService = new MongoHabitStatisticService(database);
        monthKey = DateTime.Today.ToString("yyyy-MM");
        dayKey = DateTime.Today.ToString("dd");
        daysOfWeek = ["Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"];
    }

    //This ones gonna be painful
    [Fact]
    public async Task TestGetHistoricalData()
    {
        DateTime today = DateTime.Today;

        IMongoCollection<User> users = database.GetCollection<User>("Users");
        IMongoCollection<HabitCollection> collection = database.GetCollection<HabitCollection>("HabitCollection");

        string id = ObjectId.GenerateNewId().ToString();
        string past = DateTime.Today.AddDays(-15).ToString("yyyy-MM-dd");
        string password = "asdfasdf";
        string username = "Jack";

        User user = new()
        {
            Id = id,
            Username = username,
            //Hash the password before storing in database
            Password = PasswordHasher.HashPassword(password),
            LastLoginDate = past
        };

        string habitId = ObjectId.GenerateNewId().ToString();
        Habit habit = new Habit
        {
            Id = habitId,
            Name = "Read 25 Pages",
            DateCreated = past,
            DaysActive = ["Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"],
            Type = HabitType.NUMERIC,
            Value = 25,
            ValueUnitType = "Pages"
        };

        HabitCollection habitCollection = new()
        {
            Id = id,
            ActiveHabits = [habit],
            HabitHistory = [],
            NonActiveHabits = []
        };

        await users.InsertOneAsync(user);
        await collection.InsertOneAsync(habitCollection);
        LoginResult result = await userService.Login(username, password);
        string sessionKey = result.SessionKey;
        habit.Value = 50;
        await habitService.EditHabit(sessionKey, habit);

        await habitHistoryService.SetHabitCompletion(sessionKey, today.ToString("yyyy-MM-dd"), habit!.Id!, true);
        await habitHistoryService.SetHabitCompletion(sessionKey, today.AddDays(-1).ToString("yyyy-MM-dd"), habit!.Id!, true);
        await habitHistoryService.SetHabitCompletion(sessionKey, today.AddDays(-2).ToString("yyyy-MM-dd"), habit!.Id!, true);
        await habitHistoryService.SetHabitCompletion(sessionKey, today.AddDays(-3).ToString("yyyy-MM-dd"), habit!.Id!, true);

        await habitHistoryService.SetHabitCompletion(sessionKey, today.AddDays(-5).ToString("yyyy-MM-dd"), habit!.Id!, true);
        await habitHistoryService.SetHabitCompletion(sessionKey, today.AddDays(-6).ToString("yyyy-MM-dd"), habit!.Id!, true);
        await habitHistoryService.SetHabitCompletion(sessionKey, today.AddDays(-7).ToString("yyyy-MM-dd"), habit!.Id!, true);
        await habitHistoryService.SetHabitCompletion(sessionKey, today.AddDays(-8).ToString("yyyy-MM-dd"), habit!.Id!, true);
        await habitHistoryService.SetHabitCompletion(sessionKey, today.AddDays(-9).ToString("yyyy-MM-dd"), habit!.Id!, true);
        await habitHistoryService.SetHabitCompletion(sessionKey, today.AddDays(-10).ToString("yyyy-MM-dd"), habit!.Id!, true);

        await habitHistoryService.SetHabitCompletion(sessionKey, today.AddDays(-12).ToString("yyyy-MM-dd"), habit!.Id!, true);
        await habitHistoryService.SetHabitCompletion(sessionKey, today.AddDays(-13).ToString("yyyy-MM-dd"), habit!.Id!, true);

        HistoricalData? data = await habitStatisticService.GetHistoricalData(sessionKey, habitId);

        Assert.Equal(4, data!.CurrentStreak);
        Assert.Equal(6, data!.LongestStreak);
        Assert.Equal(325, data!.TotalValueCompleted);
        Assert.Equal(12, data!.DaysCompleted);

        await habitHistoryService.SetHabitCompletion(sessionKey, today.ToString("yyyy-MM-dd"), habit!.Id!, false);
        data = await habitStatisticService.GetHistoricalData(sessionKey, habitId);

        Assert.Equal(3, data!.CurrentStreak);

    }
}