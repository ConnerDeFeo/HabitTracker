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


public class TestMongoHabit : IAsyncLifetime
{

    string dbName;
    string monthKey;
    string dayKey;
    IMongoDatabase database;
    IUserService userService;
    IHabitService habitService;
    HashSet<string> daysOfWeek;
    TestingUtils utils;

    public TestMongoHabit()
    {
        var Client = new MongoClient("mongodb://localhost:27017");
        dbName = $"TestMongoHabitHistoryService_{Guid.NewGuid().ToString()[..20]}";
        database = Client.GetDatabase(dbName);
        userService = new MongoUserService(database);
        utils = new TestingUtils(userService);
        habitService = new MongoHabitService(database);
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

    private async Task<HabitCollection> GetHabitCollection(string sessionKey)
    {
        User user = await database.GetCollection<User>("Users")
        .Find(BuilderUtils.userFilter.Exists($"SessionKeys.{sessionKey}"))
        .FirstOrDefaultAsync();
        return await database.GetCollection<HabitCollection>("HabitCollection")
        .Find(BuilderUtils.habitFilter.Eq(hc => hc.Id, user.Id))
        .FirstOrDefaultAsync();
    }

    [Fact]
    public async Task TestGetHabits()
    {
        LoginResult result = await utils.CreateUser("ConnerDeFeo1");

        List<Habit>? habits = await habitService.GetHabits(result.SessionKey, $"{monthKey}-{dayKey}");

        Assert.NotNull(habits);
        Assert.Empty(habits);
    }

    [Fact]
    public async Task TestGetHabitsInvalid()
    {
        LoginResult result = await utils.CreateUser("ConnerDeFeo2");

        List<Habit>? habits = await habitService.GetHabits("InvalidSessionKey", $"{monthKey}-{dayKey}");

        Assert.Null(habits);
    }

    [Fact]
    public async Task TestCreateHabit()
    {
        LoginResult result = await utils.CreateUser("ConnerDeFeo3");
        string sessionKey = result.SessionKey;

        Habit? habit = await habitService.CreateHabit(sessionKey, new Habit { Name = "TestHabit", DaysActive = daysOfWeek });
        List<Habit>? habits = await habitService.GetHabits(result.SessionKey, $"{monthKey}-{dayKey}");
        HabitCollection? collection = await GetHabitCollection(sessionKey);
        Habit historyHabit = collection!.HabitHistory[monthKey][dayKey]!.Habits[habits![0].Id!];

        Assert.NotNull(habit);
        Assert.Equal("TestHabit", habits![0].Name);
        Assert.NotEmpty(habits!);
        Assert.NotNull(historyHabit);
        Assert.Equal("TestHabit", historyHabit.Name);

        habit = await habitService.CreateHabit(sessionKey, new Habit { Name = "NewTestHabit" });
        collection = await GetHabitCollection(sessionKey);

        Assert.Single(collection!.HabitHistory[monthKey][dayKey]!.Habits);

    }

    [Fact]
    public async Task TestCreateHabitInvalid()
    {
        LoginResult result = await utils.CreateUser("ConnerDeFeo4");
        string sessionKey = result.SessionKey;

        Habit? habit = await habitService.CreateHabit(sessionKey, new Habit { Name = "TestHabit", DaysActive = daysOfWeek });
        await habitService.DeactivateHabit(sessionKey, habit!.Id!);
        habit = await habitService.CreateHabit(sessionKey, new Habit { Name = "TestHabit" });
        List<Habit>? habits = await habitService.GetHabits(result.SessionKey, $"{monthKey}-{dayKey}");

        Assert.Null(habit);
        Assert.Empty(habits!);

    }

    [Fact]
    public async Task TestDeleteHabit()
    {
        LoginResult result = await utils.CreateUser("ConnerDeFeo5");
        string sessionKey = result.SessionKey;
        string id = ObjectId.GenerateNewId().ToString();

        Habit habit = new Habit { Name = "TestHabit", Id = id, DaysActive = daysOfWeek };
        await habitService.CreateHabit(sessionKey, habit);
        await habitService.DeactivateHabit(sessionKey, habit.Id);
        bool deleted = await habitService.DeleteHabit(sessionKey, habit.Id);
        HabitCollection? collection = await GetHabitCollection(sessionKey);

        Assert.True(deleted);
        Assert.Empty(collection!.NonActiveHabits);


    }

    [Fact]
    public async Task TestDeleteHabitInvalid()
    {
        LoginResult result = await utils.CreateUser("ConnerDeFeo6");
        string sessionKey = result.SessionKey;

        Habit? habit = await habitService.CreateHabit(sessionKey, new Habit { Name = "TestHabit", DaysActive = daysOfWeek });
        await habitService.DeactivateHabit(sessionKey, habit!.Id!);
        bool deleted = await habitService.DeleteHabit(sessionKey, ObjectId.GenerateNewId().ToString());
        HabitCollection? collection = await GetHabitCollection(sessionKey);

        Assert.False(deleted);
        Assert.NotEmpty(collection!.NonActiveHabits);
    }

    [Fact]
    public async Task TestEditHabit()
    {
        LoginResult result = await utils.CreateUser("ConnerDeFeo7");
        string sessionKey = result.SessionKey;

        Habit? habit = await habitService.CreateHabit(sessionKey, new Habit { Name = "TestHabit", DaysActive = daysOfWeek });
        await habitService.CreateHabit(sessionKey, new Habit { Name = "1" });
        await habitService.CreateHabit(sessionKey, new Habit { Name = "2" });

        string id = habit!.Id!;
        Habit? habitAfter = await habitService.EditHabit(sessionKey, new Habit { Name = "TestHabitUpdated", Id = id });
        List<Habit>? habits = await habitService.GetHabits(result.SessionKey, $"{monthKey}-{dayKey}");
        Dictionary<string, List<Habit>>? existingHabits = await habitService.GetExistingHabits(sessionKey);

        Assert.Empty(habits!);
        Assert.Equal("TestHabitUpdated", habitAfter!.Name);
        Assert.Equal("TestHabitUpdated",existingHabits!["ActiveHabits"][0].Name);
    }

    [Fact]
    public async Task TestEditHabitInvalid()
    {
        LoginResult result = await utils.CreateUser("ConnerDeFeo8");
        string sessionKey = result.SessionKey;

        Habit? habit = await habitService.CreateHabit(sessionKey, new Habit { Name = "TestHabit", DaysActive = daysOfWeek });
        Habit? habitAfter = await habitService.EditHabit(sessionKey, new Habit { Name = "TestHabitUpdated", Id = ObjectId.GenerateNewId().ToString() });
        List<Habit>? habits = await habitService.GetHabits(result.SessionKey, $"{monthKey}-{dayKey}");
        Dictionary<string, List<Habit>>? existingHabits = await habitService.GetExistingHabits(sessionKey);

        Assert.Null(habitAfter);
        Assert.Equal("TestHabit", habits![0].Name);
        Assert.Equal("TestHabit",existingHabits!["ActiveHabits"][0].Name);
    }

    [Fact]
    public async Task TestDeactivateHabit()
    {
        LoginResult result = await utils.CreateUser("ConnerDeFeo9");
        string sessionKey = result.SessionKey;

        Habit? habit = await habitService.CreateHabit(sessionKey, new Habit { Name = "TestHabit", DaysActive = daysOfWeek });
        bool deactivated = await habitService.DeactivateHabit(sessionKey, habit!.Id!);
        HabitCollection? collection = await GetHabitCollection(sessionKey);

        Assert.True(deactivated);
        Assert.Empty(collection.ActiveHabits);
        Assert.Single(collection.NonActiveHabits);
    }
    [Fact]
    public async Task TestDeactivateHabitInvalid()
    {
        LoginResult result = await utils.CreateUser("ConnerDeFeo10");
        string sessionKey = result.SessionKey;

        Habit? habit = await habitService.CreateHabit(sessionKey, new Habit { Name = "TestHabit", DaysActive = daysOfWeek });
        bool deactivated = await habitService.DeactivateHabit(sessionKey, ObjectId.GenerateNewId().ToString());
        HabitCollection? collection = await GetHabitCollection(sessionKey);

        Assert.False(deactivated);
        Assert.Single(collection.ActiveHabits);
        Assert.Empty(collection.NonActiveHabits);
    }
    [Fact]
    public async Task TestReactivateHabit()
    {
        LoginResult result = await utils.CreateUser("ConnerDeFeo11");
        string sessionKey = result.SessionKey;

        Habit? habit = await habitService.CreateHabit(sessionKey, new Habit { Name = "TestHabit", DaysActive = daysOfWeek });
        await habitService.DeactivateHabit(sessionKey, habit!.Id!);
        bool reactivated = await habitService.ReactivateHabit(sessionKey, habit!.Id!);
        HabitCollection? collection = await GetHabitCollection(sessionKey);

        Assert.True(reactivated);
        Assert.Single(collection.ActiveHabits);
        Assert.Empty(collection.NonActiveHabits);
    }
    [Fact]
    public async Task TestReactivateHabitInvalid()
    {
        LoginResult result = await utils.CreateUser("ConnerDeFeo12");
        string sessionKey = result.SessionKey;

        Habit? habit = await habitService.CreateHabit(sessionKey, new Habit { Name = "TestHabit", DaysActive = daysOfWeek });
        await habitService.DeactivateHabit(sessionKey, habit!.Id!);
        bool reactivated = await habitService.ReactivateHabit(sessionKey, ObjectId.GenerateNewId().ToString());
        HabitCollection? collection = await GetHabitCollection(sessionKey);

        Assert.False(reactivated);
        Assert.Empty(collection.ActiveHabits);
        Assert.Single(collection.NonActiveHabits);
    }

    [Fact]
    public async Task TestGetExistingHabits()
    {
        LoginResult result = await utils.CreateUser("ConnerDeFeo13");
        string sessionKey = result.SessionKey;

        Habit? habit = await habitService.CreateHabit(sessionKey, new Habit { Name = "TestHabit" });
        await habitService.DeactivateHabit(sessionKey, habit!.Id!);
        await habitService.CreateHabit(sessionKey, new Habit { Name = "TestHabit2" });
        await habitService.CreateHabit(sessionKey, new Habit { Name = "TestHabit3" });

        Dictionary<string, List<Habit>>? existingHabits = await habitService.GetExistingHabits(sessionKey);
        List<Habit> activeHabits = existingHabits!["ActiveHabits"];
        List<Habit> nonActiveHabits = existingHabits!["NonActiveHabits"];

        Assert.Equal(2, activeHabits.Count);
        Assert.Single(nonActiveHabits);
    }

    [Fact]
    public async Task TestGetExistingHabitsInvalid()
    {
        LoginResult result = await utils.CreateUser("ConnerDeFeo14");
        string sessionKey = result.SessionKey;

        Habit? habit = await habitService.CreateHabit(sessionKey, new Habit { Name = "TestHabit" });
        await habitService.DeactivateHabit(sessionKey, habit!.Id!);
        await habitService.CreateHabit(sessionKey, new Habit { Name = "TestHabit2" });
        await habitService.CreateHabit(sessionKey, new Habit { Name = "TestHabit3" });

        Dictionary<string, List<Habit>>? existingHabits = await habitService.GetExistingHabits(ObjectId.GenerateNewId().ToString());
        Assert.Null(existingHabits);
    }
}