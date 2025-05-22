namespace Test.service;
using MongoDB.Driver;
using Server.service;
using Server.service.concrete;
using Server.model.user;
using Server.model.habit;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using MongoDB.Bson;

public class TestMongoHabitService
{

    string dateKey;
    IMongoDatabase database;
    IUserService userService;
    IHabitService habitService;

    public TestMongoHabitService()
    {
        var Client = new MongoClient("mongodb://localhost:27017");
        Client.DropDatabase("TestMongoHabitService");
        database = Client.GetDatabase("TestMongoHabitService");
        userService = new MongoUserService(database);
        habitService = new MongoHabitService(database);
        dateKey = DateTime.Today.ToString("yyyy-MM-dd");
    }

    [Fact]
    public async Task TestGetHabits()
    {
        LoginResult result = await userService.CreateUser("ConnerDeFeo", "12345678");

        List<Habit>? habits = await habitService.GetHabits(result.SessionKey);

        Assert.NotNull(habits);
        Assert.Empty(habits);
    }

    [Fact]
    public async Task TestGetHabitsInvalid()
    {
        LoginResult result = await userService.CreateUser("ConnerDeFeo", "12345678");

        List<Habit>? habits = await habitService.GetHabits("INVALID");

        Assert.Null(habits);
    }

    [Fact]
    public async Task TestCreateHabit()
    {
        LoginResult result = await userService.CreateUser("ConnerDeFeo", "12345678");
        string sessionKey = result.SessionKey;

        List<Habit>? habits = await habitService.CreateHabit(sessionKey, new Habit { Name = "TestHabit" });
        HabitCollection? collection = await habitService.GetHabitCollection(sessionKey);
        Habit historyHabit = collection!.HabitHistory[dateKey]![habits![0].Id];

        Assert.NotEmpty(habits!);
        Assert.Equal("TestHabit", habits![0].Name);
        Assert.NotNull(historyHabit);
        Assert.Equal("TestHabit", historyHabit.Name);

    }

    [Fact]
    public async Task TestDeleteHabit()
    {
        LoginResult result = await userService.CreateUser("ConnerDeFeo", "12345678");
        string sessionKey = result.SessionKey;
        string id = ObjectId.GenerateNewId().ToString();

        Habit habit = new Habit { Name = "TestHabit", Id = id };
        await habitService.CreateHabit(sessionKey, habit);
        List<Habit>? habits = await habitService.DeleteHabit(sessionKey, habit);
        HabitCollection? collection = await habitService.GetHabitCollection(sessionKey);

        Assert.Empty(habits!);
        Assert.Empty(collection!.HabitHistory[dateKey]);

    }

    [Fact]
    public async Task TestDeleteHabitInvalid()
    {
        LoginResult result = await userService.CreateUser("ConnerDeFeo", "12345678");
        string sessionKey = result.SessionKey;

        List<Habit>? habits = await habitService.CreateHabit(sessionKey, new Habit { Name = "TestHabit" });
        List<Habit>? deletedHabits = await habitService.DeleteHabit(sessionKey, new Habit { Name = "TestHabit", Id = ObjectId.GenerateNewId().ToString() });
        HabitCollection? collection = await habitService.GetHabitCollection(sessionKey);

        Assert.Null(deletedHabits);
        Assert.NotEmpty(collection!.Habits);
        Assert.NotNull(collection!.HabitHistory[dateKey][habits![0].Id]);
    }

    [Fact]
    public async Task TestEditHabit()
    {
        LoginResult result = await userService.CreateUser("ConnerDeFeo", "12345678");
        string sessionKey = result.SessionKey;

        List<Habit>? habitsBefore = await habitService.CreateHabit(sessionKey, new Habit { Name = "TestHabit" });
        await habitService.CreateHabit(sessionKey, new Habit { Name = "1" });
        await habitService.CreateHabit(sessionKey, new Habit { Name = "2" });

        string id = habitsBefore![0].Id;
        List<Habit>? habitsAfter = await habitService.EditHabit(sessionKey, new Habit { Name = "TestHabitUpdated", Id = id });
        HabitCollection? collection = await habitService.GetHabitCollection(sessionKey);

        Habit habit = habitsAfter![0];
        Assert.Equal("TestHabitUpdated", habit.Name);
        Assert.Equal("TestHabitUpdated", collection!.HabitHistory[dateKey][id].Name);
    }

    [Fact]
    public async Task TestEditHabitInvalid()
    {
        LoginResult result = await userService.CreateUser("ConnerDeFeo", "12345678");
        string sessionKey = result.SessionKey;

        await habitService.CreateHabit(sessionKey, new Habit { Name = "TestHabit" });
        await habitService.CreateHabit(sessionKey, new Habit { Name = "1" });
        await habitService.CreateHabit(sessionKey, new Habit { Name = "2" });

        string id = ObjectId.GenerateNewId().ToString();
        List<Habit>? habits = await habitService.EditHabit(sessionKey, new Habit { Name = "TestHabitUpdated", Id = id });
        HabitCollection? collection = await habitService.GetHabitCollection(sessionKey);

        Assert.Null(habits);
        Habit habit = collection!.Habits[0];
        Assert.Equal("TestHabit", habit.Name);
        Assert.Equal("TestHabit", collection!.HabitHistory[dateKey][habit.Id].Name);
    }

    [Fact]
    public async Task TestGetHabitCollection()
    {
        LoginResult result = await userService.CreateUser("Conner", "12341234");
        HabitCollection? collection = await habitService.GetHabitCollection(result.SessionKey);

        Assert.NotNull(collection);
    }

    [Fact]
    public async Task TestSetHabitCompletion()
    {
        LoginResult result = await userService.CreateUser("Conner", "12341234");
        string sessionKey = result.SessionKey;

        List<Habit>? habits = await habitService.CreateHabit(sessionKey, new Habit { Name = "TestHabit" });
        Habit habit = habits![0];
        List<Habit>? datedHabits = await habitService.SetHabitCompletion(sessionKey, DateTime.Today.ToString("yyyy-MM-dd"), habit,true);
        HabitCollection? collection = await habitService.GetHabitCollection(sessionKey);

        Assert.True(datedHabits![0].Completed);
        Assert.True(collection!.HabitHistory[DateTime.Today.ToString("yyyy-MM-dd")][habit.Id].Completed);

    }

    [Fact]
    public async Task TestSetHabitCompletionFailiure()
    {
        LoginResult result = await userService.CreateUser("Conner", "12341234");
        string sessionKey = result.SessionKey;

        List<Habit>? habits = await habitService.CreateHabit(sessionKey, new Habit { Name = "TestHabit" });
        Habit habit = habits![0];
        List<Habit>? datedHabits = await habitService.SetHabitCompletion(sessionKey, DateTime.Today.ToString("yyyy-MM-dd"), new Habit { Name = "TestHabit", Id = ObjectId.GenerateNewId().ToString() },true);
        HabitCollection? collection = await habitService.GetHabitCollection(sessionKey);

        Assert.False(datedHabits![0].Completed);
        Assert.False(collection!.HabitHistory[DateTime.Today.ToString("yyyy-MM-dd")][habit.Id].Completed);

    }

}