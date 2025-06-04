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

    string monthKey;
    string dayKey;
    IMongoDatabase database;
    IUserService userService;
    IHabitService habitService;
    HashSet<string> daysOfWeek;

    public TestMongoHabitService()
    {
        var Client = new MongoClient("mongodb://localhost:27017");
        Client.DropDatabase("TestMongoHabitService");
        database = Client.GetDatabase("TestMongoHabitService");
        userService = new MongoUserService(database);
        habitService = new MongoHabitService(database);
        monthKey = DateTime.Today.ToString("yyyy-MM");
        dayKey = DateTime.Today.ToString("dd");
        daysOfWeek = ["Sunday","Monday","Tuesday","Wednesday","Thursday","Friday","Saturday"];
    }

    private async Task<HabitCollection> GetHabitCollection(string sessionKey)
    {
        User user = await database.GetCollection<User>("Users")
        .Find(BuilderUtils.userFilter.Eq(u => u.SessionKey, sessionKey))
        .FirstOrDefaultAsync();
        return await database.GetCollection<HabitCollection>("HabitCollection")
        .Find(BuilderUtils.habitFilter.Eq(hc => hc.Id, user.Id))
        .FirstOrDefaultAsync();
    }

    [Fact]
    public async Task TestGetHabits()
    {
        LoginResult result = await userService.CreateUser("ConnerDeFeo", "12345678");

        List<Habit>? habits = await habitService.GetHabits(result.SessionKey, $"{monthKey}-{dayKey}");

        Assert.NotNull(habits);
        Assert.Empty(habits);
    }

    [Fact]
    public async Task TestGetHabitsInvalid()
    {
        LoginResult result = await userService.CreateUser("ConnerDeFeo", "12345678");

        List<Habit>? habits = await habitService.GetHabits("InvalidSessionKey",$"{monthKey}-{dayKey}");

        Assert.Null(habits);
    }

    [Fact]
    public async Task TestCreateHabit()
    {
        LoginResult result = await userService.CreateUser("ConnerDeFeo", "12345678");
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

    }

    [Fact]
    public async Task TestCreateHabitInvalid()
    {
        LoginResult result = await userService.CreateUser("ConnerDeFeo", "12345678");
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
        LoginResult result = await userService.CreateUser("ConnerDeFeo", "12345678");
        string sessionKey = result.SessionKey;
        string id = ObjectId.GenerateNewId().ToString();

        Habit habit = new Habit { Name = "TestHabit", Id = id, DaysActive = daysOfWeek};
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
        LoginResult result = await userService.CreateUser("ConnerDeFeo", "12345678");
        string sessionKey = result.SessionKey;

        Habit? habit = await habitService.CreateHabit(sessionKey, new Habit { Name = "TestHabit", DaysActive = daysOfWeek});
        await habitService.DeactivateHabit(sessionKey, habit!.Id!);
        bool deleted = await habitService.DeleteHabit(sessionKey, ObjectId.GenerateNewId().ToString());
        HabitCollection? collection = await  GetHabitCollection(sessionKey);

        Assert.False(deleted);
        Assert.NotEmpty(collection!.NonActiveHabits);
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
        List<Habit>? habits = await habitService.GetHabits(result.SessionKey,$"{monthKey}-{dayKey}");
        HabitCollection? collection = await  GetHabitCollection(sessionKey);

        Assert.Equal("TestHabitUpdated", habits![0].Name);
        Assert.Equal("TestHabitUpdated", habitAfter!.Name);
        Assert.Equal("TestHabitUpdated", collection!.HabitHistory[monthKey][dayKey].Habits[id].Name);
    }

    [Fact]
    public async Task TestEditHabitInvalid()
    {
        LoginResult result = await userService.CreateUser("ConnerDeFeo", "12345678");
        string sessionKey = result.SessionKey;

        Habit? habit = await habitService.CreateHabit(sessionKey, new Habit { Name = "TestHabit", DaysActive = daysOfWeek});
        Habit? habitAfter = await habitService.EditHabit(sessionKey, new Habit { Name = "TestHabitUpdated", Id = ObjectId.GenerateNewId().ToString()});
        List<Habit>? habits = await habitService.GetHabits(result.SessionKey,$"{monthKey}-{dayKey}");
        HabitCollection? collection = await  GetHabitCollection(sessionKey);

        Assert.Null(habitAfter);
        Assert.Equal("TestHabit", habits![0].Name);
        Assert.Equal("TestHabit", collection!.HabitHistory[monthKey][dayKey].Habits[habit!.Id!].Name);
    }

}