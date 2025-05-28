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

        Habit? habit = await habitService.CreateHabit(sessionKey, new Habit { Name = "TestHabit" });
        List<Habit>? habits = await habitService.GetHabits(result.SessionKey);
        HabitCollection? collection = await habitService.GetHabitCollection(sessionKey);
        Habit historyHabit = collection!.HabitHistory[dateKey]!.Habits[habits![0].Id!];

        Assert.NotNull(habit);
        Assert.Equal("TestHabit", habits![0].Name);
        Assert.NotEmpty(habits!);
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
        bool deleted = await habitService.DeleteHabit(sessionKey, habit.Id);
        List<Habit>? habits = await habitService.GetHabits(result.SessionKey);
        HabitCollection? collection = await habitService.GetHabitCollection(sessionKey);

        Assert.True(deleted);
        Assert.Empty(habits!);
        Assert.Empty(collection!.HabitHistory[dateKey].Habits);

    }

    [Fact]
    public async Task TestDeleteHabitInvalid()
    {
        LoginResult result = await userService.CreateUser("ConnerDeFeo", "12345678");
        string sessionKey = result.SessionKey;

        Habit? habit = await habitService.CreateHabit(sessionKey, new Habit { Name = "TestHabit" });
        bool deleted = await habitService.DeleteHabit(sessionKey, ObjectId.GenerateNewId().ToString());
        List<Habit>? habits = await habitService.GetHabits(result.SessionKey);
        HabitCollection? collection = await habitService.GetHabitCollection(sessionKey);

        Assert.False(deleted);
        Assert.NotEmpty(habits!);
        Assert.NotEmpty(collection!.Habits); 
        Assert.NotNull(collection!.HabitHistory[dateKey].Habits[habits![0].Id!]);
    }

    [Fact]
    public async Task TestEditHabit()
    {
        LoginResult result = await userService.CreateUser("ConnerDeFeo", "12345678");
        string sessionKey = result.SessionKey;

        Habit? habit = await habitService.CreateHabit(sessionKey, new Habit { Name = "TestHabit" });
        await habitService.CreateHabit(sessionKey, new Habit { Name = "1" });
        await habitService.CreateHabit(sessionKey, new Habit { Name = "2" });

        string id = habit!.Id!;
        Habit? habitAfter = await habitService.EditHabit(sessionKey, new Habit { Name = "TestHabitUpdated", Id = id });
        List<Habit>? habits = await habitService.GetHabits(result.SessionKey);
        HabitCollection? collection = await habitService.GetHabitCollection(sessionKey);

        Assert.Equal("TestHabitUpdated", habits![0].Name);
        Assert.Equal("TestHabitUpdated", habitAfter!.Name);
        Assert.Equal("TestHabitUpdated", collection!.HabitHistory[dateKey].Habits[id].Name);
    }

    [Fact]
    public async Task TestEditHabitInvalid()
    {
        LoginResult result = await userService.CreateUser("ConnerDeFeo", "12345678");
        string sessionKey = result.SessionKey;

        Habit? habit = await habitService.CreateHabit(sessionKey, new Habit { Name = "TestHabit" });
        Habit? habitAfter = await habitService.EditHabit(sessionKey, new Habit { Name = "TestHabitUpdated" });
        List<Habit>? habits = await habitService.GetHabits(result.SessionKey);
        HabitCollection? collection = await habitService.GetHabitCollection(sessionKey);

        Assert.Null(habitAfter);
        Assert.Equal("TestHabit", habits![0].Name);
        Assert.Equal("TestHabit", collection!.HabitHistory[dateKey].Habits[habit!.Id!].Name);
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

        Habit? habit = await habitService.CreateHabit(sessionKey, new Habit { Name = "TestHabit" });
        bool completed = await habitService.SetHabitCompletion(sessionKey, DateTime.Today.ToString("yyyy-MM-dd"), habit!.Id!,true);
        HabitCollection? collection = await habitService.GetHabitCollection(sessionKey);

        Assert.True(completed);
        Assert.True(collection!.HabitHistory[DateTime.Today.ToString("yyyy-MM-dd")].Habits[habit!.Id!].Completed);

    }

    [Fact]
    public async Task TestSetHabitCompletionFailiure()
    {
        LoginResult result = await userService.CreateUser("Conner", "12341234");
        string sessionKey = result.SessionKey;

        Habit? habit = await habitService.CreateHabit(sessionKey, new Habit { Name = "TestHabit" });
        bool completed = await habitService.SetHabitCompletion(sessionKey, DateTime.Today.ToString("yyyy-MM-dd"), ObjectId.GenerateNewId().ToString() ,true);
        HabitCollection? collection = await habitService.GetHabitCollection(sessionKey);

        Assert.False(completed);
        Assert.False(collection!.HabitHistory[DateTime.Today.ToString("yyyy-MM-dd")].Habits[habit!.Id!].Completed);

    }

}