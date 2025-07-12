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


public class TestMongoSocialData : IAsyncLifetime
{

    string dbName;
    string monthKey;
    string dayKey;
    IMongoDatabase database;
    IUserService userService;
    IHabitService habitService;
    ISocialDataService socialDataService;
    IFriendModificationService friendModificationService;
    HashSet<string> daysOfWeek;
    TestingUtils utils;

    public TestMongoSocialData()
    {
        var Client = new MongoClient("mongodb://localhost:27017");
        dbName = $"TestMongoFriendService_{Guid.NewGuid().ToString()[..20]}";
        database = Client.GetDatabase(dbName);
        userService = new MongoUserService(database);
        utils = new TestingUtils(userService);
        habitService = new MongoHabitService(database);
        socialDataService = new MongoSocialDataService(database);
        friendModificationService = new MongoFriendModificationService(database);
        monthKey = DateTime.UtcNow.ToString("yyyy-MM");
        dayKey = DateTime.UtcNow.ToString("dd");
        daysOfWeek = new HashSet<string> { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" };
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync()
    {
        var Client = new MongoClient("mongodb://localhost:27017");
        await Client.DropDatabaseAsync(dbName);
    }

    [Fact]
    public async Task TestGetFriends()
    {
        LoginResult? userLoginResult = await utils.CreateUser("Conner10");
        LoginResult? friendLoginResult = await utils.CreateUser("Friend10");

        string userSessionKey = userLoginResult!.SessionKey;
        string friendSessionKey = friendLoginResult!.SessionKey;

        UserDto? friend = await userService.GetUser(friendSessionKey);
        UserDto? user = await userService.GetUser(userSessionKey);
        Assert.NotNull(user);
        Assert.NotNull(friend);
        string friendUsername = friend.Username;
        string userUsername = user.Username;

        await friendModificationService.SendFriendRequest(userSessionKey, friendUsername);
        await friendModificationService.AcceptFriendRequest(friendSessionKey, userUsername);

        Dictionary<string, string>? friendsResult = await socialDataService.GetFriends(userSessionKey);
        Assert.NotNull(friendsResult);
        Assert.Single(friendsResult);
    }

    [Fact]
    public async Task TestGetFriendsFaliure()
    {
        LoginResult userLoginResult = await utils.CreateUser("Conner11");
        LoginResult friendLoginResult = await utils.CreateUser("Friend11");

        string userSessionKey = userLoginResult.SessionKey;
        string friendSessionKey = friendLoginResult.SessionKey;

        UserDto? friend = await userService.GetUser(friendSessionKey);
        UserDto? user = await userService.GetUser(userSessionKey);
        Assert.NotNull(user);
        Assert.NotNull(friend);
        string friendUsername = friend.Username;
        string userUsername = user.Username;

        Dictionary<string, string>? friendsResult = await socialDataService.GetFriends(userSessionKey);
        Assert.NotNull(friendsResult);
        Assert.Empty(friendsResult);

        friendsResult = await socialDataService.GetFriends(ObjectId.GenerateNewId().ToString());
        Assert.Null(friendsResult);
    }

    [Fact]
    public async Task TestGetFriendProfile()
    {
        LoginResult? userLoginResult = await utils.CreateUser("Conner12");
        LoginResult? friendLoginResult = await utils.CreateUser("Friend12");

        string userSessionKey = userLoginResult!.SessionKey;
        string friendSessionKey = friendLoginResult!.SessionKey;

        UserDto? friend = await userService.GetUser(friendSessionKey);
        UserDto? user = await userService.GetUser(userSessionKey);
        Assert.NotNull(user);
        Assert.NotNull(friend);
        string friendUsername = friend.Username;
        string userUsername = user.Username;

        await friendModificationService.SendFriendRequest(userSessionKey, friendUsername);
        await friendModificationService.AcceptFriendRequest(friendSessionKey, userUsername);
        await habitService.CreateHabit(friendSessionKey, new Habit
        {
            Name = "Test Habit",
            DaysActive = daysOfWeek,
        });
        Profile? profile = await socialDataService.GetProfile(userSessionKey, friendUsername);
        Assert.NotNull(profile);
        Assert.Single(profile.CurrentHabits);
        Assert.Single(profile.CurrentMonthHabitsCompleted);
        Assert.Equal("Test Habit", profile.CurrentHabits[0].Name);
    }

    [Fact]
    public async Task TestGetFriendProfileFailure()
    {
        LoginResult? userLoginResult = await utils.CreateUser("Conner13");
        LoginResult? friendLoginResult = await utils.CreateUser("Friend13");

        string userSessionKey = userLoginResult!.SessionKey;
        string friendSessionKey = friendLoginResult!.SessionKey;

        UserDto? friend = await userService.GetUser(friendSessionKey);
        UserDto? user = await userService.GetUser(userSessionKey);
        Assert.NotNull(user);
        Assert.NotNull(friend);
        string friendUsername = friend.Username;
        string userUsername = user.Username;

        await friendModificationService.SendFriendRequest(userSessionKey, friendUsername);
        await habitService.CreateHabit(friendSessionKey, new Habit
        {
            Name = "Test Habit",
            DaysActive = daysOfWeek,
        });
        Profile? profile = await socialDataService.GetProfile(userSessionKey, friendUsername);
        Assert.Null(profile);
    }

    [Fact]
    public async Task TestFindUser()
    {
        LoginResult? userLoginResult = await utils.CreateUser("Conner14");
        await utils.CreateUser("person1");
        await utils.CreateUser("erson2");
        await utils.CreateUser("3person3");
        await utils.CreateUser("234perso");
        await utils.CreateUser("234person1");

        string userSessionKey = userLoginResult!.SessionKey;

        Dictionary<string, string>? found = await socialDataService.FindUser(userSessionKey, "person");

        Assert.NotNull(found);
        Assert.Equal(3, found.Count);

        found = await socialDataService.FindUser(userSessionKey, "Conner14");
        Assert.NotNull(found);
        Assert.Empty(found);
    }


    [Fact]
    public async Task TestFindUserFaliure()
    {
        LoginResult? userLoginResult = await utils.CreateUser("Conner15");
        await utils.CreateUser("person1");
        await utils.CreateUser("erson2");
        await utils.CreateUser("3person3");
        await utils.CreateUser("234perso");
        await utils.CreateUser("234person1");

        Dictionary<string, string>? found = await socialDataService.FindUser(ObjectId.GenerateNewId().ToString(), "person");
        Assert.Null(found);
    }

    [Fact]
    public async Task TestGetRandomUsers()
    {
        LoginResult? userLoginResult = await utils.CreateUser("Conner16");
        await utils.CreateUser("person1");
        await utils.CreateUser("erson2");
        await utils.CreateUser("3person3");

        string userSessionKey = userLoginResult!.SessionKey;

        Dictionary<string, string>? users = await socialDataService.GetRandomUsers(userSessionKey);

        Assert.NotNull(users);
        Assert.Equal(3, users.Count);
    }

    [Fact]
    public async Task TestGetRandomUsersFaliure()
    {
        LoginResult? userLoginResult = await utils.CreateUser("Conner17");
        await utils.CreateUser("person1");
        await utils.CreateUser("erson2");
        await utils.CreateUser("3person3");
        await utils.CreateUser("234perso");
        await utils.CreateUser("234person1");

        Dictionary<string, string>? users = await socialDataService.GetRandomUsers(ObjectId.GenerateNewId().ToString());
        Assert.Null(users);
    }
}