namespace Test.service;
using MongoDB.Driver;
using Server.service.interfaces;
using Server.service.utils;
using Server.service.concrete;
using Server.model.user;
using Server.model.habit;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using MongoDB.Bson;

public class TestMongoFriendService : IAsyncLifetime
{

    string dbName;
    string monthKey;
    string dayKey;
    IMongoDatabase database;
    IUserService userService;
    IHabitService habitService;
    IFriendService friendService;
    HashSet<string> daysOfWeek;

    public TestMongoFriendService()
    {
        var Client = new MongoClient("mongodb://localhost:27017");
        dbName = $"TestFriendService_{Guid.NewGuid().ToString()[..20]}";
        database = Client.GetDatabase(dbName);
        userService = new MongoUserService(database);
        habitService = new MongoHabitService(database);
        friendService = new MongoFriendService(database);
        monthKey = DateTime.Today.ToString("yyyy-MM");
        dayKey = DateTime.Today.ToString("dd");
        daysOfWeek = ["Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"];
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync()
    {
        var Client = new MongoClient("mongodb://localhost:27017");
        await Client.DropDatabaseAsync(dbName);
    }

    [Fact]
    public async Task TestGetProfileHabits()
    {
        LoginResult result = await userService.CreateUser("Conner1", "12341234");
        string sessionKey = result.SessionKey;

        await habitService.CreateHabit(sessionKey, new Habit { Name = "TestHabit", DaysActive = daysOfWeek });
        await habitService.CreateHabit(sessionKey, new Habit { Name = "TestHabit2", DaysActive = daysOfWeek });
        await habitService.CreateHabit(sessionKey, new Habit { Name = "TestHabit3", DaysActive = daysOfWeek });

        List<ProfileHabit> habits = await friendService.GetProfileHabits(sessionKey,"");
    }
    
    [Fact]
    public async Task TestGetProfileHabitsFaliure()
    {
        LoginResult result = await userService.CreateUser("Conner2s", "12341234");
        string sessionKey = result.SessionKey;

        Habit? habit = await habitService.CreateHabit(sessionKey, new Habit { Name = "TestHabit", DaysActive = daysOfWeek });
    }
}