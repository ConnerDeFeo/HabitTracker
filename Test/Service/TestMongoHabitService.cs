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

    public TestMongoHabitService()
    {
        var Client = new MongoClient("mongodb://localhost:27017");
        Client.DropDatabase("TestMongoHabitService");
        database = Client.GetDatabase("TestMongoHabitService");
        userService = new MongoUserService(database);
        habitService = new MongoHabitService(database);
        monthKey = DateTime.Today.ToString("yyyy-MM");
        dayKey = DateTime.Today.ToString("dd");
    }

    [Fact]
    public async Task TestGetHabits()
    {
        LoginResult result = await userService.CreateUser("ConnerDeFeo", "12345678");

        List<Habit>? habits = await habitService.GetHabits(result.SessionKey, DateTime.Today.ToString("yyyy-MM-dd"));

        Assert.NotNull(habits);
        Assert.Empty(habits);
    }

    [Fact]
    public async Task TestGetHabitsInvalid()
    {
        LoginResult result = await userService.CreateUser("ConnerDeFeo", "12345678");

        List<Habit>? habits = await habitService.GetHabits("INVALID",DateTime.Today.ToString("yyyy-MM-dd"));

        Assert.Null(habits);
    }

    [Fact]
    public async Task TestCreateHabit()
    {
        LoginResult result = await userService.CreateUser("ConnerDeFeo", "12345678");
        string sessionKey = result.SessionKey;

        Habit? habit = await habitService.CreateHabit(sessionKey, new Habit { Name = "TestHabit" });
        List<Habit>? habits = await habitService.GetHabits(result.SessionKey,DateTime.Today.ToString("yyyy-MM-dd"));
        HabitCollection? collection = await habitService.GetHabitCollection(sessionKey);
        Habit historyHabit = collection!.HabitHistory[monthKey][dayKey]!.Habits[habits![0].Id!];

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
        List<Habit>? habits = await habitService.GetHabits(result.SessionKey,DateTime.Today.ToString("yyyy-MM-dd"));
        HabitCollection? collection = await habitService.GetHabitCollection(sessionKey);

        Assert.True(deleted);
        Assert.Empty(habits!);
        Assert.Empty(collection!.HabitHistory[monthKey][dayKey].Habits);

    }

    [Fact]
    public async Task TestDeleteHabitInvalid()
    {
        LoginResult result = await userService.CreateUser("ConnerDeFeo", "12345678");
        string sessionKey = result.SessionKey;

        Habit? habit = await habitService.CreateHabit(sessionKey, new Habit { Name = "TestHabit" });
        bool deleted = await habitService.DeleteHabit(sessionKey, ObjectId.GenerateNewId().ToString());
        List<Habit>? habits = await habitService.GetHabits(result.SessionKey,DateTime.Today.ToString("yyyy-MM-dd"));
        HabitCollection? collection = await habitService.GetHabitCollection(sessionKey);

        Assert.False(deleted);
        Assert.NotEmpty(habits!);
        Assert.NotEmpty(collection!.Habits);
        Assert.NotNull(collection!.HabitHistory[monthKey][dayKey].Habits[habits![0].Id!]);
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
        List<Habit>? habits = await habitService.GetHabits(result.SessionKey,DateTime.Today.ToString("yyyy-MM-dd"));
        HabitCollection? collection = await habitService.GetHabitCollection(sessionKey);

        Assert.Equal("TestHabitUpdated", habits![0].Name);
        Assert.Equal("TestHabitUpdated", habitAfter!.Name);
        Assert.Equal("TestHabitUpdated", collection!.HabitHistory[monthKey][dayKey].Habits[id].Name);
    }

    [Fact]
    public async Task TestEditHabitInvalid()
    {
        LoginResult result = await userService.CreateUser("ConnerDeFeo", "12345678");
        string sessionKey = result.SessionKey;

        Habit? habit = await habitService.CreateHabit(sessionKey, new Habit { Name = "TestHabit" });
        Habit? habitAfter = await habitService.EditHabit(sessionKey, new Habit { Name = "TestHabitUpdated" });
        List<Habit>? habits = await habitService.GetHabits(result.SessionKey,DateTime.Today.ToString("yyyy-MM-dd"));
        HabitCollection? collection = await habitService.GetHabitCollection(sessionKey);

        Assert.Null(habitAfter);
        Assert.Equal("TestHabit", habits![0].Name);
        Assert.Equal("TestHabit", collection!.HabitHistory[monthKey][dayKey].Habits[habit!.Id!].Name);
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
        bool completed = await habitService.SetHabitCompletion(sessionKey, DateTime.Today.ToString("yyyy-MM-dd"), habit!.Id!, true);
        HabitCollection? collection = await habitService.GetHabitCollection(sessionKey);

        Assert.True(completed);
        Assert.True(collection!.HabitHistory[monthKey][dayKey].Habits[habit!.Id!].Completed);
    }

    [Fact]
    public async Task TestSetHabitCompletionFailiure()
    {
        LoginResult result = await userService.CreateUser("Conner", "12341234");
        string sessionKey = result.SessionKey;

        Habit? habit = await habitService.CreateHabit(sessionKey, new Habit { Name = "TestHabit" });
        bool completed = await habitService.SetHabitCompletion(sessionKey, DateTime.Today.ToString("yyyy-MM-dd"), ObjectId.GenerateNewId().ToString(), true);
        HabitCollection? collection = await habitService.GetHabitCollection(sessionKey);

        Assert.False(completed);
        Assert.False(collection!.HabitHistory[monthKey][dayKey].Habits[habit!.Id!].Completed);

    }

    [Fact]
    public async Task TestAllHabitsCompleted()
    {
        LoginResult result = await userService.CreateUser("Conner", "12341234");
        string sessionKey = result.SessionKey;
        string today = DateTime.Today.ToString("yyyy-MM-dd");

        Habit? habit = await habitService.CreateHabit(sessionKey, new Habit { Name = "TestHabit" });
        await habitService.SetHabitCompletion(sessionKey, today, habit!.Id!, true);
        HabitCollection? collection = await habitService.GetHabitCollection(sessionKey);

        HistoricalDate date = collection!.HabitHistory[monthKey][dayKey];

        Assert.True(date.AllHabitsCompleted);

        //Remove habit
        await habitService.DeleteHabit(sessionKey, habit!.Id!);

        collection = await habitService.GetHabitCollection(sessionKey);
        date = collection!.HabitHistory[monthKey][dayKey];

        Assert.True(date.AllHabitsCompleted);

        //Add a new habit
        await habitService.CreateHabit(sessionKey, habit!);

        collection = await habitService.GetHabitCollection(sessionKey);
        date = collection!.HabitHistory[monthKey][dayKey];

        Assert.False(date.AllHabitsCompleted);

        //Complete then add new habit
        await habitService.SetHabitCompletion(sessionKey, today, habit!.Id!, true);
        await habitService.CreateHabit(sessionKey, new Habit { Name = "TestHabit2" });

        collection = await habitService.GetHabitCollection(sessionKey);
        date = collection!.HabitHistory[monthKey][dayKey];

        Assert.False(date.AllHabitsCompleted);

    }

    [Fact]
    public async Task TestGetHabitHistoryByMonth()
    {
        IMongoCollection<User> users = database.GetCollection<User>("Users");
        IMongoCollection<HabitCollection> collection = database.GetCollection<HabitCollection>("HabitCollection");

        string id = ObjectId.GenerateNewId().ToString();
        string today = DateTime.Today.ToString("yyyy-MM-dd");
        string password = "asdfasdf";
        string username = "Jack";

        User user = new()
        {
            Id = id,
            Username = username,
            //Hash the password before storing in database
            Password = PasswordHasher.HashPassword(password),
            LastLoginDate = today
        };

        HabitCollection habitCollection = new()
        {
            Id = id,
            Habits = [],
            HabitHistory = [],
            DeletedHabits = []
        };

        habitCollection.HabitHistory["0000-00"] = [];
        habitCollection.HabitHistory["0000-00"]["00"] = new(); 
        habitCollection.HabitHistory["0000-00"]["01"] = new(); 
        habitCollection.HabitHistory["0000-01"] = [];
        habitCollection.HabitHistory["0000-01"]["00"] = new();
        habitCollection.HabitHistory["0001-01"] = [];
        habitCollection.HabitHistory["0001-01"]["00"] = new();
        habitCollection.HabitHistory["0001-00"] = [];
        habitCollection.HabitHistory["0001-00"]["00"] = new();

        await users.InsertOneAsync(user);
        await collection.InsertOneAsync(habitCollection);
        LoginResult result = await userService.Login(username, password);
        string sessionKey = result.SessionKey;

        Dictionary<string, HistoricalDate>? datedHabits = await habitService.GetHabitHistoryByMonth(sessionKey, "0000-00");
        Assert.Equal(2, datedHabits!.Count);

        datedHabits = await habitService.GetHabitHistoryByMonth(sessionKey, "0000-01");
        Assert.Single(datedHabits!);

        datedHabits = await habitService.GetHabitHistoryByMonth(sessionKey, "0001-01");
        Assert.Single(datedHabits!);

        datedHabits = await habitService.GetHabitHistoryByMonth(sessionKey, "0001-00");
        Assert.Single(datedHabits!);
    }

}