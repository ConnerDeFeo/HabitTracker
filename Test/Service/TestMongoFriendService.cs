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


public class TestMongoFriendService : IAsyncLifetime
{

    string dbName;
    string monthKey;
    string dayKey;
    IMongoDatabase database;
    IUserService userService;
    IHabitService habitService;
    IFriendService friendService;
    HashSet<string> daysOfWeek;

    public TestMongoFriendService()
    {
        var Client = new MongoClient("mongodb://localhost:27017");
        dbName = $"TestMongoFriendService_{Guid.NewGuid().ToString()[..20]}";
        database = Client.GetDatabase(dbName);
        userService = new MongoUserService(database);
        habitService = new MongoHabitService(database);
        friendService = new MongoFriendService(database);
        monthKey = DateTime.Today.ToString("yyyy-MM");
        dayKey = DateTime.Today.ToString("dd");
        daysOfWeek = new HashSet<string> { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" };
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync()
    {
        var Client = new MongoClient("mongodb://localhost:27017");
        await Client.DropDatabaseAsync(dbName);
    }

    [Fact]
    public async Task TestSendFriendRequest()
    {
        LoginResult? userLoginResult = await userService.CreateUser("Conner", "12341234");
        LoginResult? friendLoginResult = await userService.CreateUser("Friend", "12341234");

        string userSessionKey = userLoginResult!.SessionKey;
        string friendSessionKey = friendLoginResult!.SessionKey;

        UserDto? friend = await userService.GetUser(friendSessionKey);
        UserDto? user = await userService.GetUser(userSessionKey);
        Assert.NotNull(user);
        Assert.NotNull(friend);

        string friendUsername = friend.Username;
        bool addFriendResult = await friendService.SendFriendRequest(userSessionKey, friendUsername);

        Assert.True(addFriendResult);

        UserDto? postUserDto = await userService.GetUser(userSessionKey);
        Assert.NotNull(postUserDto);
        Assert.Empty(postUserDto.Friends);
        Assert.Empty(postUserDto.FriendRequests);
        Assert.Single(postUserDto.FriendRequestsSent);

        UserDto? postFriendDto = await userService.GetUser(friendSessionKey);
        Assert.NotNull(postFriendDto);
        Assert.Single(postFriendDto.FriendRequests);
        Assert.Empty(postFriendDto.Friends);
        Assert.Empty(postFriendDto.FriendRequestsSent);

    }

    [Fact]
    public async Task TestSendFriendRequestFaliure()
    {
        LoginResult? userLoginResult = await userService.CreateUser("Conner2", "12341234");
        LoginResult? friendLoginResult = await userService.CreateUser("Friend2", "12341234");

        string userSessionKey = userLoginResult!.SessionKey;
        string friendSessionKey = friendLoginResult!.SessionKey;

        UserDto? friend = await userService.GetUser(friendSessionKey);
        UserDto? user = await userService.GetUser(userSessionKey);
        Assert.NotNull(user);
        Assert.NotNull(friend);

        string friendUsername = friend.Username;
        bool addFriendResult = await friendService.SendFriendRequest(userSessionKey, friendUsername);

        addFriendResult = await friendService.SendFriendRequest(userSessionKey, friendUsername);
        Assert.False(addFriendResult);

        UserDto? postUserDto = await userService.GetUser(userSessionKey);
        Assert.NotNull(postUserDto);
        Assert.Empty(postUserDto.Friends);
        Assert.Empty(postUserDto.FriendRequests);
        Assert.Single(postUserDto.FriendRequestsSent);

        UserDto? postFriendDto = await userService.GetUser(friendSessionKey);
        Assert.NotNull(postFriendDto);
        Assert.Single(postFriendDto.FriendRequests);
        Assert.Empty(postFriendDto.Friends);
        Assert.Empty(postFriendDto.FriendRequestsSent);

    }

    [Fact]
    public async Task TestUnSendFriendRequest()
    {
        LoginResult? userLoginResult = await userService.CreateUser("Conner3", "12341234");
        LoginResult? friendLoginResult = await userService.CreateUser("Friend3", "12341234");

        string userSessionKey = userLoginResult!.SessionKey;
        string friendSessionKey = friendLoginResult!.SessionKey;

        UserDto? friend = await userService.GetUser(friendSessionKey);
        UserDto? user = await userService.GetUser(userSessionKey);
        Assert.NotNull(user);
        Assert.NotNull(friend);
        string friendUsername = friend.Username;

        await friendService.SendFriendRequest(userSessionKey, friendUsername);
        bool unSendFriendResult = await friendService.UnSendFriendRequest(userSessionKey, friendUsername);

        Assert.True(unSendFriendResult);

        UserDto? postUserDto = await userService.GetUser(userSessionKey);
        Assert.NotNull(postUserDto);
        Assert.Empty(postUserDto.Friends);
        Assert.Empty(postUserDto.FriendRequests);
        Assert.Empty(postUserDto.FriendRequestsSent);

        UserDto? postFriendDto = await userService.GetUser(friendSessionKey);
        Assert.NotNull(postFriendDto);
        Assert.Empty(postFriendDto.FriendRequests);
        Assert.Empty(postFriendDto.Friends);
        Assert.Empty(postFriendDto.FriendRequestsSent);

    }

    [Fact]
    public async Task TestUnSendFriendRequestFaliure()
    {
        LoginResult? userLoginResult = await userService.CreateUser("Conner4", "12341234");
        LoginResult? friendLoginResult = await userService.CreateUser("Friend4", "12341234");

        string userSessionKey = userLoginResult!.SessionKey;
        string friendSessionKey = friendLoginResult!.SessionKey;

        UserDto? friend = await userService.GetUser(friendSessionKey);
        UserDto? user = await userService.GetUser(userSessionKey);
        Assert.NotNull(user);
        Assert.NotNull(friend);

        string friendUsername = friend.Username;
        bool unSendFriendResult = await friendService.UnSendFriendRequest(userSessionKey, friendUsername);

        Assert.False(unSendFriendResult);

        UserDto? postUserDto = await userService.GetUser(userSessionKey);
        Assert.NotNull(postUserDto);
        Assert.Empty(postUserDto.Friends);
        Assert.Empty(postUserDto.FriendRequests);
        Assert.Empty(postUserDto.FriendRequestsSent);

        UserDto? postFriendDto = await userService.GetUser(friendSessionKey);
        Assert.NotNull(postFriendDto);
        Assert.Empty(postFriendDto.FriendRequests);
        Assert.Empty(postFriendDto.Friends);
        Assert.Empty(postFriendDto.FriendRequestsSent);

    }

    [Fact]
    public async Task TestAcceptFriendRequest()
    {
        LoginResult? userLoginResult = await userService.CreateUser("Conner5", "12341234");
        LoginResult? friendLoginResult = await userService.CreateUser("Friend5", "12341234");

        string userSessionKey = userLoginResult!.SessionKey;
        string friendSessionKey = friendLoginResult!.SessionKey;

        UserDto? friend = await userService.GetUser(friendSessionKey);
        UserDto? user = await userService.GetUser(userSessionKey);
        Assert.NotNull(user);
        Assert.NotNull(friend);
        string friendUsername = friend.Username;
        string userUsername = user.Username;

        await friendService.SendFriendRequest(userSessionKey, friendUsername);
        Dictionary<string, string?>? acceptFriendResult = await friendService.AcceptFriendRequest(friendSessionKey, userUsername);

        Assert.NotNull(acceptFriendResult);
        Assert.Single(acceptFriendResult);
    }

    [Fact]
    public async Task TestAcceptFriendRequestFaliure()
    {
        LoginResult? userLoginResult = await userService.CreateUser("Conner6", "12341234");
        LoginResult? friendLoginResult = await userService.CreateUser("Friend6", "12341234");

        string userSessionKey = userLoginResult!.SessionKey;
        string friendSessionKey = friendLoginResult!.SessionKey;

        UserDto? friend = await userService.GetUser(friendSessionKey);
        UserDto? user = await userService.GetUser(userSessionKey);
        Assert.NotNull(user);
        Assert.NotNull(friend);
        string friendUsername = friend.Username;
        string userUsername = user.Username;
        Dictionary<string, string?>? acceptFriendResult = await friendService.AcceptFriendRequest(friendSessionKey, userUsername);

        Assert.Null(acceptFriendResult);
    }

    [Fact]
    public async Task TestRemoveFriend()
    {
        LoginResult? userLoginResult = await userService.CreateUser("Conner7", "12341234");
        LoginResult? friendLoginResult = await userService.CreateUser("Friend7", "12341234");

        string userSessionKey = userLoginResult!.SessionKey;
        string friendSessionKey = friendLoginResult!.SessionKey;

        UserDto? friend = await userService.GetUser(friendSessionKey);
        UserDto? user = await userService.GetUser(userSessionKey);
        Assert.NotNull(user);
        Assert.NotNull(friend);
        string friendUsername = friend.Username;
        string userUsername = user.Username;

        await friendService.SendFriendRequest(userSessionKey, friendUsername);
        await friendService.AcceptFriendRequest(friendSessionKey, userUsername);
        Dictionary<string, string?>? removeFriendResult = await friendService.RemoveFriend(userSessionKey, friendUsername);

        Assert.NotNull(removeFriendResult);
        Assert.Empty(removeFriendResult);
    }

    [Fact]
    public async Task TestRemoveFriendFaliure()
    {
        LoginResult? userLoginResult = await userService.CreateUser("Conner7", "12341234");
        LoginResult? friendLoginResult = await userService.CreateUser("Friend7", "12341234");

        string userSessionKey = userLoginResult!.SessionKey;
        string friendSessionKey = friendLoginResult!.SessionKey;

        UserDto? friend = await userService.GetUser(friendSessionKey);
        UserDto? user = await userService.GetUser(userSessionKey);
        Assert.NotNull(user);
        Assert.NotNull(friend);
        string friendUsername = friend.Username;

        Dictionary<string, string?>? removeFriendResult = await friendService.RemoveFriend(userSessionKey, friendUsername);
        Assert.Null(removeFriendResult);
    }

    [Fact]
    public async Task TestRejectFriendRequest()
    {
        LoginResult? userLoginResult = await userService.CreateUser("Conner8", "12341234");
        LoginResult? friendLoginResult = await userService.CreateUser("Friend8", "12341234");

        string userSessionKey = userLoginResult!.SessionKey;
        string friendSessionKey = friendLoginResult!.SessionKey;

        UserDto? friend = await userService.GetUser(friendSessionKey);
        UserDto? user = await userService.GetUser(userSessionKey);
        Assert.NotNull(user);
        Assert.NotNull(friend);
        string friendUsername = friend.Username;
        string userUsername = user.Username;

        await friendService.SendFriendRequest(userSessionKey, friendUsername);
        bool rejectFriendResult = await friendService.RejectFriendRequest(friendSessionKey, userUsername);
        Assert.True(rejectFriendResult);

        UserDto? postUserDto = await userService.GetUser(userSessionKey);
        Assert.NotNull(postUserDto);
        Assert.Empty(postUserDto.Friends);
        Assert.Empty(postUserDto.FriendRequests);
        Assert.Empty(postUserDto.FriendRequestsSent);

        UserDto? postFriendDto = await userService.GetUser(friendSessionKey);
        Assert.NotNull(postFriendDto);
        Assert.Empty(postFriendDto.FriendRequests);
        Assert.Empty(postFriendDto.Friends);
        Assert.Empty(postFriendDto.FriendRequestsSent);

    }

    [Fact]
    public async Task TestRejectFriendRequestFaliure()
    {
        LoginResult? userLoginResult = await userService.CreateUser("Conner9", "12341234");
        LoginResult? friendLoginResult = await userService.CreateUser("Friend9", "12341234");

        string userSessionKey = userLoginResult!.SessionKey;
        string friendSessionKey = friendLoginResult!.SessionKey;

        UserDto? friend = await userService.GetUser(friendSessionKey);
        UserDto? user = await userService.GetUser(userSessionKey);
        Assert.NotNull(user);
        Assert.NotNull(friend);
        string friendUsername = friend.Username;
        string userUsername = user.Username;
        bool rejectFriendResult = await friendService.RejectFriendRequest(friendSessionKey, userUsername);

        Assert.False(rejectFriendResult);
    }

    [Fact]
    public async Task TestGetFriends()
    {
        LoginResult? userLoginResult = await userService.CreateUser("Conner10", "12341234");
        LoginResult? friendLoginResult = await userService.CreateUser("Friend10", "12341234");

        string userSessionKey = userLoginResult!.SessionKey;
        string friendSessionKey = friendLoginResult!.SessionKey;

        UserDto? friend = await userService.GetUser(friendSessionKey);
        UserDto? user = await userService.GetUser(userSessionKey);
        Assert.NotNull(user);
        Assert.NotNull(friend);
        string friendUsername = friend.Username;
        string userUsername = user.Username;

        await friendService.SendFriendRequest(userSessionKey, friendUsername);
        await friendService.AcceptFriendRequest(friendSessionKey, userUsername);

        Dictionary<string, string?>? friendsResult = await friendService.GetFriends(userSessionKey);
        Assert.NotNull(friendsResult);
        Assert.Single(friendsResult);
    }

    [Fact]
    public async Task TestGetFriendsFaliure()
    {
        LoginResult? userLoginResult = await userService.CreateUser("Conner11", "12341234");
        LoginResult? friendLoginResult = await userService.CreateUser("Friend11", "12341234");

        string userSessionKey = userLoginResult!.SessionKey;
        string friendSessionKey = friendLoginResult!.SessionKey;

        UserDto? friend = await userService.GetUser(friendSessionKey);
        UserDto? user = await userService.GetUser(userSessionKey);
        Assert.NotNull(user);
        Assert.NotNull(friend);
        string friendUsername = friend.Username;
        string userUsername = user.Username;

        Dictionary<string, string?>? friendsResult = await friendService.GetFriends(userSessionKey);
        Assert.NotNull(friendsResult);
        Assert.Empty(friendsResult);

        friendsResult = await friendService.GetFriends(ObjectId.GenerateNewId().ToString());
        Assert.Null(friendsResult);
    }

    [Fact]
    public async Task TestGetFriendProfile()
    {
        LoginResult? userLoginResult = await userService.CreateUser("Conner12", "12341234");
        LoginResult? friendLoginResult = await userService.CreateUser("Friend12", "12341234");

        string userSessionKey = userLoginResult!.SessionKey;
        string friendSessionKey = friendLoginResult!.SessionKey;

        UserDto? friend = await userService.GetUser(friendSessionKey);
        UserDto? user = await userService.GetUser(userSessionKey);
        Assert.NotNull(user);
        Assert.NotNull(friend);
        string friendUsername = friend.Username;
        string userUsername = user.Username;

        await friendService.SendFriendRequest(userSessionKey, friendUsername);
        await friendService.AcceptFriendRequest(friendSessionKey, userUsername);
        await habitService.CreateHabit(friendSessionKey, new Habit
        {
            Name = "Test Habit",
            DaysActive = daysOfWeek,
        });
        ProfileHabits? profileHabits = await friendService.GetFriendProfile(userSessionKey, friendUsername);
        Assert.NotNull(profileHabits);
        Assert.Single(profileHabits.CurrentHabits);
        Assert.Single(profileHabits.CurrentMonthHabitsCompleted);
        Assert.Equal("Test Habit", profileHabits.CurrentHabits[0].Name);
    }

    [Fact]
    public async Task TestGetFriendProfileFailure()
    {
        LoginResult? userLoginResult = await userService.CreateUser("Conner13", "12341234");
        LoginResult? friendLoginResult = await userService.CreateUser("Friend13", "12341234");

        string userSessionKey = userLoginResult!.SessionKey;
        string friendSessionKey = friendLoginResult!.SessionKey;

        UserDto? friend = await userService.GetUser(friendSessionKey);
        UserDto? user = await userService.GetUser(userSessionKey);
        Assert.NotNull(user);
        Assert.NotNull(friend);
        string friendUsername = friend.Username;
        string userUsername = user.Username;

        await friendService.SendFriendRequest(userSessionKey, friendUsername);
        await habitService.CreateHabit(friendSessionKey, new Habit
        {
            Name = "Test Habit",
            DaysActive = daysOfWeek,
        });
        ProfileHabits? profileHabits = await friendService.GetFriendProfile(userSessionKey, friendUsername);
        Assert.Null(profileHabits);
    }

    [Fact]
    public async Task TestFindUser()
    {
        LoginResult? userLoginResult = await userService.CreateUser("Conner14", "12341234");
        await userService.CreateUser("person1", "12341234");
        await userService.CreateUser("erson2", "12341234");
        await userService.CreateUser("3person3", "12341234");
        await userService.CreateUser("234perso", "12341234");
        await userService.CreateUser("234person1", "12341234");

        string userSessionKey = userLoginResult!.SessionKey;

        Dictionary<string, string?>? found = await friendService.FindUser(userSessionKey, "person");

        Assert.NotNull(found);
        Assert.Equal(3, found.Count);

        found = await friendService.FindUser(userSessionKey, "Conner14");
        Assert.NotNull(found);
        Assert.Empty(found);
    }


    [Fact]
    public async Task TestFindUserFaliure()
    {
        LoginResult? userLoginResult = await userService.CreateUser("Conner14", "12341234");
        await userService.CreateUser("person1", "12341234");
        await userService.CreateUser("erson2", "12341234");
        await userService.CreateUser("3person3", "12341234");
        await userService.CreateUser("234perso", "12341234");
        await userService.CreateUser("234person1", "12341234");

        Dictionary<string, string?>? found = await friendService.FindUser(ObjectId.GenerateNewId().ToString(), "person");
        Assert.Null(found);
    }

    [Fact]
    public async Task TestGetRandomUsers()
    {
        LoginResult? userLoginResult = await userService.CreateUser("Conner14", "12341234");
        await userService.CreateUser("person1", "12341234");
        await userService.CreateUser("erson2", "12341234");
        await userService.CreateUser("3person3", "12341234");

        string userSessionKey = userLoginResult!.SessionKey;

        Dictionary<string, string?>? users = await friendService.GetRandomUsers(userSessionKey);

        Assert.NotNull(users);
        Assert.Equal(3, users.Count);
    }

    [Fact]
    public async Task TestGetRandomUsersFaliure()
    {
        LoginResult? userLoginResult = await userService.CreateUser("Conner14", "12341234");
        await userService.CreateUser("person1", "12341234");
        await userService.CreateUser("erson2", "12341234");
        await userService.CreateUser("3person3", "12341234");
        await userService.CreateUser("234perso", "12341234");
        await userService.CreateUser("234person1", "12341234");

        Dictionary<string, string?>? users = await friendService.GetRandomUsers(ObjectId.GenerateNewId().ToString());
        Assert.Null(users);
    }
}