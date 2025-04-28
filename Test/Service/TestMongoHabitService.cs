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
        Client.DropDatabase("HabitTracker");
        var database = Client.GetDatabase("HabitTracker");
        userService = new MongoUserService(database);
        habitService = new MongoHabitService(database);
    }

    [Fact]
    public async Task TestGetHabits(){
        LoginResult result = await userService.CreateUser("ConnerDeFeo","12345678");

        List<Habit>? habits = await habitService.GetHabits(result.SessionKey);

        User? user = await userService.GetUser(result.SessionKey);
        Assert.NotNull(user?.Habits);
    }
}