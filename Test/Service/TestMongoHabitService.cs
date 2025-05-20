namespace Test.service;
using MongoDB.Driver;
using Server.service;
using Server.service.concrete;
using Server.model;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using MongoDB.Bson;

public class TestMongoHabitService{

    IUserService userService;
    IHabitService habitService;

    public TestMongoHabitService(){
        var Client = new MongoClient("mongodb://localhost:27017");
        Client.DropDatabase("TestMongoHabitService");
        var database = Client.GetDatabase("TestMongoHabitService");
        userService = new MongoUserService(database);
        habitService = new MongoHabitService(database);
    }

    [Fact]
    public async Task TestGetHabits(){
        LoginResult result = await userService.CreateUser("ConnerDeFeo","12345678");

        List<Habit>? habits = await habitService.GetHabits(result.SessionKey);

        Assert.NotNull(habits);
        Assert.Empty(habits);
    }

    [Fact]
    public async Task TestGetHabitsInvalid(){
        LoginResult result = await userService.CreateUser("ConnerDeFeo","12345678");

        List<Habit>? habits = await habitService.GetHabits("INVALID");

        Assert.Null(habits);
    }

    [Fact]
    public async Task TestCreateHabit(){
        LoginResult result = await userService.CreateUser("ConnerDeFeo","12345678");
        string sessionKey = result.SessionKey;

        List<Habit>? inMemoryHabits = await habitService.CreateHabit(sessionKey, new Habit{Name="TestHabit"});
        List<Habit>? habits = await habitService.GetHabits(sessionKey);

        Assert.NotEmpty(inMemoryHabits!);
        Assert.NotEmpty(habits!);
        Assert.Equal("TestHabit",habits![0].Name);
        Assert.Equal(habits![0].Name,inMemoryHabits![0].Name);
    }

    [Fact]
    public async Task TestDeleteHabit()
    {
        // Create a user
        LoginResult result = await userService.CreateUser("ConnerDeFeo", "12345678");
        string sessionKey = result.SessionKey;
        Habit habit = new Habit { Name = "TestHabit", Id = ObjectId.GenerateNewId().ToString() };

        // Create a habit
        await habitService.CreateHabit(sessionKey, habit);
        
        List<Habit>? habits = await habitService.DeleteHabit(sessionKey, habit);

        Assert.Empty(habits!);
    }

    [Fact]
    public async Task TestDeleteHabitInvalid(){
        LoginResult result = await userService.CreateUser("ConnerDeFeo","12345678");
        string sessionKey = result.SessionKey;

        await habitService.CreateHabit(sessionKey, new Habit { Name = "TestHabit", Id = ObjectId.GenerateNewId().ToString() });
        List<Habit>? habits = await habitService.DeleteHabit(sessionKey, new Habit{Name="TestHabit"});

        Assert.NotEmpty(habits!);
    }

    [Fact]
    public async Task TestEditHabit(){
        LoginResult result = await userService.CreateUser("ConnerDeFeo","12345678");
        string sessionKey = result.SessionKey;
        string id=ObjectId.GenerateNewId().ToString();

        await habitService.CreateHabit(sessionKey, new Habit{Name="TestHabit",Id=id});
        await habitService.CreateHabit(sessionKey, new Habit{Name="1"});
        await habitService.CreateHabit(sessionKey, new Habit{Name="2"});
        List<Habit>? habits = await habitService.EditHabit(sessionKey, new Habit{Name="TestHabitUpdated", Id=id});

        Habit habit = habits![0];

        Assert.Equal("TestHabitUpdated",habit.Name);
    }

    [Fact]
    public async Task TestEditHabitInvalid(){
        LoginResult result = await userService.CreateUser("ConnerDeFeo","12345678");
        string sessionKey = result.SessionKey;

        await habitService.CreateHabit(sessionKey, new Habit{Name="TestHabit"});
        await habitService.CreateHabit(sessionKey, new Habit{Name="1"});
        await habitService.CreateHabit(sessionKey, new Habit{Name="2"});
        List<Habit>? habits = await habitService.EditHabit(sessionKey, new Habit{Name="TestHabitUpdated", Id=ObjectId.GenerateNewId().ToString()});

        Assert.Equal("TestHabitUpdated", habits![0].Name);
    }

}