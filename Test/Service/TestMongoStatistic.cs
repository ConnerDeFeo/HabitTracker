namespace Test.service;
using MongoDB.Driver;
using Server.service.interfaces;
using Server.service.utils;
using Server.service.concrete;
using Server.model.user;
using Server.model.habit;
using Server.dtos;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using MongoDB.Bson;

public class TestMongoHabitStatistic : IAsyncLifetime
{
    string dbName;
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
        dbName = $"TestMongoHabitStatisticService_{Guid.NewGuid().ToString()[..30]}";
        database = Client.GetDatabase(dbName);
        userService = new MongoUserService(database);
        habitService = new MongoHabitService(database);
        habitHistoryService = new MongoHabitHistoryService(database);
        habitStatisticService = new MongoHabitStatisticService(database);
        monthKey = DateTime.UtcNow.ToString("yyyy-MM");
        dayKey = DateTime.UtcNow.ToString("dd");
        daysOfWeek = ["Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"];
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync()
    {
        var Client = new MongoClient("mongodb://localhost:27017");
        await Client.DropDatabaseAsync(dbName);
    }

    //This ones gonna be painful
    [Fact]
    public async Task TestGetHistoricalData()
    {
        DateTime today = DateTime.UtcNow;

        IMongoCollection<User> users = database.GetCollection<User>("Users");
        IMongoCollection<HabitCollection> collection = database.GetCollection<HabitCollection>("HabitCollection");

        string id = ObjectId.GenerateNewId().ToString();
        string past = DateTime.UtcNow.AddDays(-15).ToString("yyyy-MM-dd");
        string password = "asdfasdf";
        string username = "Jack1";

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
        LoginResult result = await userService.Login(new LoginRequest { Username = username, Password = password, DeviceId="1234"});
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

        Assert.NotNull(data);
        Assert.Equal(4, data.CurrentStreak);
        Assert.Equal(6, data.LongestStreak);
        Assert.Equal(325, data.TotalValueCompleted);

        await habitHistoryService.SetHabitCompletion(sessionKey, today.ToString("yyyy-MM-dd"), habit!.Id!, false);
        data = await habitStatisticService.GetHistoricalData(sessionKey, habitId);

        Assert.Equal(3, data!.CurrentStreak);

    }

    [Fact]
    public async Task TestGetTotalValuesByMonth()
    {
        DateTime date = new DateTime(2024,9,3);

        IMongoCollection<User> users = database.GetCollection<User>("Users");
        IMongoCollection<HabitCollection> collection = database.GetCollection<HabitCollection>("HabitCollection");

        string id = ObjectId.GenerateNewId().ToString();
        string past = date.AddYears(-2).ToString("yyyy-MM-dd");
        string password = "asdfasdf";
        string username = "Jack2";

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
        LoginResult result = await userService.Login(new LoginRequest { Username = username, Password = password, DeviceId="1234"});
        string sessionKey = result.SessionKey;

        //Only these 9 should be counted towards being complete
        await habitHistoryService.SetHabitCompletion(sessionKey, date.ToString("yyyy-MM-dd"), habit!.Id!, true);
        await habitHistoryService.SetHabitCompletion(sessionKey, date.AddMonths(-1).ToString("yyyy-MM-dd"), habit!.Id!, true);
        await habitHistoryService.SetHabitCompletion(sessionKey, date.AddMonths(-2).ToString("yyyy-MM-dd"), habit!.Id!, true);
        await habitHistoryService.SetHabitCompletion(sessionKey, date.AddMonths(-3).ToString("yyyy-MM-dd"), habit!.Id!, true);
        await habitHistoryService.SetHabitCompletion(sessionKey, date.AddMonths(-4).ToString("yyyy-MM-dd"), habit!.Id!, true);
        await habitHistoryService.SetHabitCompletion(sessionKey, date.AddMonths(-5).ToString("yyyy-MM-dd"), habit!.Id!, true);
        await habitHistoryService.SetHabitCompletion(sessionKey, date.AddMonths(-6).ToString("yyyy-MM-dd"), habit!.Id!, true);
        await habitHistoryService.SetHabitCompletion(sessionKey, date.AddMonths(-7).ToString("yyyy-MM-dd"), habit!.Id!, true);
        await habitHistoryService.SetHabitCompletion(sessionKey, date.AddMonths(-8).ToString("yyyy-MM-dd"), habit!.Id!, true);

        await habitHistoryService.SetHabitCompletion(sessionKey, date.AddMonths(-9).ToString("yyyy-MM-dd"), habit!.Id!, true);
        await habitHistoryService.SetHabitCompletion(sessionKey, date.AddMonths(-10).ToString("yyyy-MM-dd"), habit!.Id!, true);
        await habitHistoryService.SetHabitCompletion(sessionKey, date.AddMonths(-11).ToString("yyyy-MM-dd"), habit!.Id!, true);

        Dictionary<string, int>? valuesPerMonth = await habitStatisticService.GetTotalValuesByMonth(sessionKey, habitId, 2024);

        Assert.NotNull(valuesPerMonth);
        Assert.Equal(12,valuesPerMonth.Count);
        int total = 0;
        foreach (int num in valuesPerMonth.Values)
            total += num;

        Assert.Equal(225, total);
    }
}