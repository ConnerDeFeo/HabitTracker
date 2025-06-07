namespace Test.service;
using MongoDB.Driver;
using Server.service;
using Server.service.concrete;
using Server.model.user;
using Server.model.habit;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using MongoDB.Bson;

public class TestMongoHabitHistoryService
{
    string monthKey;
    string dayKey;
    IMongoDatabase database;
    IUserService userService;
    IHabitService habitService;
    IHabitHistoryService habitHistoryService;
    HashSet<string> daysOfWeek;

    public TestMongoHabitHistoryService()
    {
        var Client = new MongoClient("mongodb://localhost:27017");
        Client.DropDatabase("TestMongoHabitHistoryService");
        database = Client.GetDatabase("TestMongoHabitHistoryService");
        userService = new MongoUserService(database);
        habitHistoryService = new MongoHabitHistoryService(database);
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
    public async Task TestSetHabitCompletion()
    {
        LoginResult result = await userService.CreateUser("Conner", "12341234");
        string sessionKey = result.SessionKey;

        Habit? habit = await habitService.CreateHabit(sessionKey, new Habit { Name = "TestHabit", DaysActive = daysOfWeek });
        bool completed = await habitHistoryService.SetHabitCompletion(sessionKey, $"{monthKey}-{dayKey}", habit!.Id!, true);
        HabitCollection? collection = await GetHabitCollection(sessionKey);

        Assert.True(completed);
        Assert.True(collection!.HabitHistory[monthKey][dayKey].Habits[habit!.Id!].Completed);
    }

    [Fact]
    public async Task TestSetHabitCompletionFailiure()
    {
        LoginResult result = await userService.CreateUser("Conner", "12341234");
        string sessionKey = result.SessionKey;

        Habit? habit = await habitService.CreateHabit(sessionKey, new Habit { Name = "TestHabit" });
        bool completed = await habitHistoryService.SetHabitCompletion(sessionKey, $"{monthKey}-{dayKey}", habit!.Id!, true);
        HabitCollection? collection = await GetHabitCollection(sessionKey);

        Assert.False(completed);

        habit = await habitService.CreateHabit(sessionKey, new Habit { Name = "NewTestHabit", DaysActive = daysOfWeek });
        completed = await habitHistoryService.SetHabitCompletion(sessionKey, $"{monthKey}-{dayKey}", ObjectId.GenerateNewId().ToString(), true);
        collection = await GetHabitCollection(sessionKey);

        Assert.False(completed);
        Assert.False(collection.HabitHistory[monthKey][dayKey].Habits[habit!.Id!].Completed);
    }

    [Fact]
    public async Task TestAllHabitsCompleted()
    {
        LoginResult result = await userService.CreateUser("Conner", "12341234");
        string sessionKey = result.SessionKey;
        string today = DateTime.Today.ToString("yyyy-MM-dd");

        Habit? habit = await habitService.CreateHabit(sessionKey, new Habit { Name = "TestHabit", DaysActive = daysOfWeek });
        await habitHistoryService.SetHabitCompletion(sessionKey, today, habit!.Id!, true);
        HabitCollection? collection = await GetHabitCollection(sessionKey);
        HistoricalDate date = collection!.HabitHistory[monthKey][dayKey];
        Assert.True(date.AllHabitsCompleted);

        //Remove habit
        await habitService.DeactivateHabit(sessionKey, habit!.Id!);
        collection = await GetHabitCollection(sessionKey);
        date = collection!.HabitHistory[monthKey][dayKey];
        Assert.True(date.AllHabitsCompleted);

        //Add a new habit
        habit = await habitService.CreateHabit(sessionKey, new Habit { Name = "NewTestHabit", DaysActive = daysOfWeek });
        collection = await GetHabitCollection(sessionKey);
        date = collection!.HabitHistory[monthKey][dayKey];
        Assert.False(date.AllHabitsCompleted);

        //Complete then add new habit
        await habitHistoryService.SetHabitCompletion(sessionKey, today, habit!.Id!, true);
        habit = await habitService.CreateHabit(sessionKey, new Habit { Name = "NewNewTestHabit", DaysActive = daysOfWeek });
        collection = await GetHabitCollection(sessionKey);
        date = collection!.HabitHistory[monthKey][dayKey];
        Assert.False(date.AllHabitsCompleted);

        await habitService.EditHabit(sessionKey, new Habit { Name = "NewNewTestHabit", Id=habit!.Id });
        collection = await GetHabitCollection(sessionKey);
        date = collection!.HabitHistory[monthKey][dayKey];
        Assert.True(date.AllHabitsCompleted);

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
            ActiveHabits = [],
            HabitHistory = [],
            NonActiveHabits = []
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

        Dictionary<string, HistoricalDate>? datedHabits = await habitHistoryService.GetHabitHistoryByMonth(sessionKey, "0000-00");
        Assert.Equal(2, datedHabits!.Count);

        datedHabits = await habitHistoryService.GetHabitHistoryByMonth(sessionKey, "0000-01");
        Assert.Single(datedHabits!);

        datedHabits = await habitHistoryService.GetHabitHistoryByMonth(sessionKey, "0001-01");
        Assert.Single(datedHabits!);

        datedHabits = await habitHistoryService.GetHabitHistoryByMonth(sessionKey, "0001-00");
        Assert.Single(datedHabits!);
        
        
    }
}