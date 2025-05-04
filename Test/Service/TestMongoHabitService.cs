namespace Test.service;
using MongoDB.Driver;
using Server.service;
using Server.service.concrete;
using Server.model;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

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
    public async Task TestCreateHabit(){
        LoginResult result = await userService.CreateUser("ConnerDeFeo","12345678");
        string sessionKey = result.SessionKey;

        bool created = await habitService.CreateHabit(sessionKey, "TestHabit");
        Assert.True(created);

        List<Habit>? habits = await habitService.GetHabits(sessionKey);

        Assert.NotEmpty(habits!);
    }
}