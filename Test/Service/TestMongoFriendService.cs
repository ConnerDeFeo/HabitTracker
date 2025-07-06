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
        LoginResult? userLoginResult = await userService.CreateUser("Conner3", "12341234");
        LoginResult? friendLoginResult = await userService.CreateUser("Friend3", "12341234");

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
        LoginResult? userLoginResult = await userService.CreateUser("Conner3", "12341234");
        LoginResult? friendLoginResult = await userService.CreateUser("Friend3", "12341234");

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

}