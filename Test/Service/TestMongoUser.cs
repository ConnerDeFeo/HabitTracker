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

public class TestMongoUser : IAsyncLifetime
{
    string dbName;
    IMongoDatabase database;
    IUserService userService;
    IHabitService habitService;
    IHabitHistoryService habitHistoryService;
    TestingUtils utils;
    public TestMongoUser()
    {
        dbName = $"TestMongoHabitHistoryService_{Guid.NewGuid().ToString()[..20]}";
        var client = new MongoClient("mongodb://localhost:27017");
        database = client.GetDatabase(dbName);
        userService = new MongoUserService(database);
        habitService = new MongoHabitService(database);
        utils = new TestingUtils(userService);
        habitHistoryService = new MongoHabitHistoryService(database);
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
    public async Task TestGetUser()
    {
        LoginResult result = await utils.CreateUser("ConnerDeFeo1");

        UserDto? user = await userService.GetUser(result.SessionKey);

        Assert.Equal("ConnerDeFeo1", user!.Username);
    }

    [Fact]
    public async Task TestGetUserInvalid()
    {
        UserDto? invalid = await userService.GetUser("HEHEHEHA");

        Assert.Null(invalid);
    }

    [Fact]
    public async Task TestCreateUser()
    {
        LoginResult result = await utils.CreateUser("ConnerDeFeo2");

        string sessionKey = result.SessionKey;

        UserDto? user = await userService.GetUser(result.SessionKey);

        Assert.Equal("ConnerDeFeo2", user!.Username);
        Assert.Equal(DateTime.Today.ToString("yyyy-MM-dd"), user!.DateCreated);

        LoginResult Result = await utils.CreateUser("ConnerDeFeo2");

        Assert.Equal("", Result.SessionKey);

    }

    [Fact]
    public async Task TestCreateUserFalse()
    {
        await utils.CreateUser("ConnerDeFeo3");

        LoginResult Result = await utils.CreateUser("ConnerDeFeo3");

        Assert.Equal("", Result.SessionKey);
    }

    [Fact]
    public async Task TestLogin()
    {
        LoginResult result = await utils.CreateUser("ConnerDeFeo4");

        LoginResult Result = await userService.Login(new LoginRequest { Username = "ConnerDeFeo4", Password = "12345678", DeviceId = "1234" });
        Assert.NotNull(Result.SessionKey);
        Assert.NotEqual("", Result.SessionKey);
        LoginResult Result2 = await userService.Login(new LoginRequest { Username = "ConnerDeFeo4", Password = "12345678", DeviceId = "1111" });
        Assert.NotNull(Result2.SessionKey);
        Assert.NotEqual("", Result2.SessionKey);

        User user = await database.GetCollection<User>("Users")
        .Find(BuilderUtils.userFilter.Exists($"SessionKeys.{Result2.SessionKey}"))
        .FirstOrDefaultAsync();

        Dictionary<string, string> sessionKeys = user.SessionKeys;

        Assert.Equal(2, sessionKeys.Count);
        Assert.True(sessionKeys.ContainsKey(Result.SessionKey));
        Assert.True(sessionKeys.ContainsKey(Result2.SessionKey));
        Assert.Equal("1234",sessionKeys[Result.SessionKey]);
        Assert.Equal("1111",sessionKeys[Result2.SessionKey]);
    

    }

    [Fact]
    public async Task TestLoginUpdatesPreviousDates()
    {
        IMongoCollection<User> users = database.GetCollection<User>("Users");
        IMongoCollection<HabitCollection> collection = database.GetCollection<HabitCollection>("HabitCollection");

        string id = ObjectId.GenerateNewId().ToString();
        string past = DateTime.Today.AddDays(-5).ToString("yyyy-MM-dd");
        string password = "asdfasdf";
        string username = "Jack";

        User user = new()
        {
            Id = id,
            Username = username,
            //Hash the password before storing in database
            Password = PasswordHasher.HashPassword(password),
            LastLoginDate = past
        };

        HabitCollection habitCollection = new()
        {
            Id = id,
            ActiveHabits = [],
            HabitHistory = [],
            NonActiveHabits = []
        };

        await users.InsertOneAsync(user);
        await collection.InsertOneAsync(habitCollection);
        LoginResult result = await userService.Login(new LoginRequest { Username = "Jack", Password = "asdfasdf", DeviceId="1234"});

        HabitCollection? habitCollectionUpdated = await GetHabitCollection(result.SessionKey);

        //in case i test at the begining of a month
        int total = 0;
        foreach (var dayDict in habitCollectionUpdated!.HabitHistory.Values)
            total += dayDict.Values.Count;


        Assert.Equal(6, total);

    }

    [Fact]
    public async Task TestLoginFaliure()
    {
        await utils.CreateUser("ConnerDeFeo5");

        LoginResult result = await userService.Login(new LoginRequest { Username = "Jack", Password = "suk", DeviceId="1234"});
        Assert.Equal("", result.SessionKey);
    }

    [Fact]
    public async Task TestLogout()
    {
        LoginResult result = await utils.CreateUser("ConnerDeFeo6");

        bool logedOut = await userService.Logout(result.SessionKey);

        Assert.True(logedOut);
    }

    [Fact]
    public async Task TestLogoutFaliure()
    {
        LoginResult result = await utils.CreateUser("ConnerDeFeo6");

        bool logedIn = await userService.Logout("");

        Assert.False(logedIn);
    }

    [Fact]
    public async Task TestGetProfile()
    {
        LoginResult result = await utils.CreateUser("ConnerDeFeo8");
        string sessionKey = result.SessionKey;

        HashSet<string> daysActive = new() { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" };

        Habit? habit = await habitService.CreateHabit(sessionKey, new Habit { Name = "TestHabit", DaysActive = daysActive });
        await habitService.CreateHabit(sessionKey, new Habit { Name = "TestHabit2", DaysActive = daysActive });
        await habitService.CreateHabit(sessionKey, new Habit { Name = "TestHabit3", DaysActive = daysActive });

        Profile? habits = await userService.GetProfile(sessionKey);
        Assert.NotNull(habits);
        Assert.Equal(3, habits.CurrentHabits.Count);
        Assert.Equal(0, habits.CurrentHabits[0].CurrentStreak);
        Assert.Equal(0, habits.CurrentHabits[1].CurrentStreak);
        Assert.Equal(0, habits.CurrentHabits[2].CurrentStreak);

        await habitHistoryService.SetHabitCompletion(sessionKey, $"{DateTime.Today:yyyy-MM-dd}", habit!.Id!, true);
        habits = await userService.GetProfile(sessionKey);
        Assert.Equal(1, habits!.CurrentHabits[0].CurrentStreak);
    }
    
    [Fact]
    public async Task TestGetprofileFaliure()
    { 
        LoginResult result = await utils.CreateUser("ConnerDeFeo9");
        string sessionKey = result.SessionKey;


        Habit? habit = await habitService.CreateHabit(sessionKey, new Habit { Name = "TestHabit"});
        await habitService.CreateHabit(sessionKey, new Habit { Name = "TestHabit2"});
        await habitService.CreateHabit(sessionKey, new Habit { Name = "TestHabit3" });

        Profile? habits = await userService.GetProfile("sessionKey");
        Assert.Null(habits);
    }

}
