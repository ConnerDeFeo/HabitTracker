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


public class TestMongoFriendModificationService : IAsyncLifetime
{

    string dbName;
    string monthKey;
    string dayKey;
    IMongoDatabase database;
    IUserService userService;
    IFriendModificationService friendModificationService;
    HashSet<string> daysOfWeek;
    TestingUtils utils;

    public TestMongoFriendModificationService()
    {
        var Client = new MongoClient("mongodb://localhost:27017");
        dbName = $"TestMongoFriendService_{Guid.NewGuid().ToString()[..20]}";
        database = Client.GetDatabase(dbName);
        userService = new MongoUserService(database);
        utils = new TestingUtils(userService);
        friendModificationService = new MongoFriendModificationService(database);
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
        LoginResult? userLoginResult = await utils.CreateUser("Conner");
        LoginResult? friendLoginResult = await utils.CreateUser("Friend");

        string userSessionKey = userLoginResult!.SessionKey;
        string friendSessionKey = friendLoginResult!.SessionKey;

        UserDto? friend = await userService.GetUser(friendSessionKey);
        UserDto? user = await userService.GetUser(userSessionKey);
        Assert.NotNull(user);
        Assert.NotNull(friend);

        string friendUsername = friend.Username;
        bool addFriendResult = await friendModificationService.SendFriendRequest(userSessionKey, friendUsername);

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
        LoginResult? userLoginResult = await utils.CreateUser("Conner2");
        LoginResult? friendLoginResult = await utils.CreateUser("Friend2");

        string userSessionKey = userLoginResult!.SessionKey;
        string friendSessionKey = friendLoginResult!.SessionKey;

        UserDto? friend = await userService.GetUser(friendSessionKey);
        UserDto? user = await userService.GetUser(userSessionKey);
        Assert.NotNull(user);
        Assert.NotNull(friend);

        string friendUsername = friend.Username;
        bool addFriendResult = await friendModificationService.SendFriendRequest(userSessionKey, friendUsername);

        addFriendResult = await friendModificationService.SendFriendRequest(userSessionKey, friendUsername);
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
        LoginResult? userLoginResult = await utils.CreateUser("Conner3");
        LoginResult? friendLoginResult = await utils.CreateUser("Friend3");

        string userSessionKey = userLoginResult!.SessionKey;
        string friendSessionKey = friendLoginResult!.SessionKey;

        UserDto? friend = await userService.GetUser(friendSessionKey);
        UserDto? user = await userService.GetUser(userSessionKey);
        Assert.NotNull(user);
        Assert.NotNull(friend);
        string friendUsername = friend.Username;

        await friendModificationService.SendFriendRequest(userSessionKey, friendUsername);
        bool unSendFriendResult = await friendModificationService.UnSendFriendRequest(userSessionKey, friendUsername);

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
        LoginResult? userLoginResult = await utils.CreateUser("Conner4");
        LoginResult? friendLoginResult = await utils.CreateUser("Friend4");

        string userSessionKey = userLoginResult!.SessionKey;
        string friendSessionKey = friendLoginResult!.SessionKey;

        UserDto? friend = await userService.GetUser(friendSessionKey);
        UserDto? user = await userService.GetUser(userSessionKey);
        Assert.NotNull(user);
        Assert.NotNull(friend);

        string friendUsername = friend.Username;
        bool unSendFriendResult = await friendModificationService.UnSendFriendRequest(userSessionKey, friendUsername);

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
        LoginResult? userLoginResult = await utils.CreateUser("Conner5");
        LoginResult? friendLoginResult = await utils.CreateUser("Friend5");

        string userSessionKey = userLoginResult!.SessionKey;
        string friendSessionKey = friendLoginResult!.SessionKey;

        UserDto? friend = await userService.GetUser(friendSessionKey);
        UserDto? user = await userService.GetUser(userSessionKey);
        Assert.NotNull(user);
        Assert.NotNull(friend);
        string friendUsername = friend.Username;
        string userUsername = user.Username;

        await friendModificationService.SendFriendRequest(userSessionKey, friendUsername);
        Dictionary<string, string>? acceptFriendResult = await friendModificationService.AcceptFriendRequest(friendSessionKey, userUsername);

        Assert.NotNull(acceptFriendResult);
        Assert.Single(acceptFriendResult);

        friend = await userService.GetUser(friendSessionKey);
        user = await userService.GetUser(userSessionKey);

        Assert.NotNull(user);
        Assert.NotNull(friend);

        Assert.Single(user.Friends);
        Assert.Single(friend.Friends);
        Assert.Empty(friend.FriendRequests);
        Assert.Empty(user.FriendRequests);
        Assert.Empty(friend.FriendRequestsSent);
        Assert.Empty(user.FriendRequestsSent);
        
    }

    [Fact]
    public async Task TestAcceptFriendRequestFaliure()
    {
        LoginResult? userLoginResult = await utils.CreateUser("Conner6");
        LoginResult? friendLoginResult = await utils.CreateUser("Friend6");

        string userSessionKey = userLoginResult!.SessionKey;
        string friendSessionKey = friendLoginResult!.SessionKey;

        UserDto? friend = await userService.GetUser(friendSessionKey);
        UserDto? user = await userService.GetUser(userSessionKey);
        Assert.NotNull(user);
        Assert.NotNull(friend);
        string friendUsername = friend.Username;
        string userUsername = user.Username;
        Dictionary<string, string>? acceptFriendResult = await friendModificationService.AcceptFriendRequest(friendSessionKey, userUsername);

        Assert.Null(acceptFriendResult);
    }

    [Fact]
    public async Task TestRemoveFriend()
    {
        LoginResult? userLoginResult = await utils.CreateUser("Conner7");
        LoginResult? friendLoginResult = await utils.CreateUser("Friend7");

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
        Dictionary<string, string>? removeFriendResult = await friendModificationService.RemoveFriend(userSessionKey, friendUsername);

        Assert.NotNull(removeFriendResult);
        Assert.Empty(removeFriendResult);
    }

    [Fact]
    public async Task TestRemoveFriendFaliure()
    {
        LoginResult? userLoginResult = await utils.CreateUser("Conner7");
        LoginResult? friendLoginResult = await utils.CreateUser("Friend7");

        string userSessionKey = userLoginResult!.SessionKey;
        string friendSessionKey = friendLoginResult!.SessionKey;

        UserDto? friend = await userService.GetUser(friendSessionKey);
        UserDto? user = await userService.GetUser(userSessionKey);
        Assert.NotNull(user);
        Assert.NotNull(friend);
        string friendUsername = friend.Username;

        Dictionary<string, string>? removeFriendResult = await friendModificationService.RemoveFriend(userSessionKey, friendUsername);
        Assert.Null(removeFriendResult);
    }

    [Fact]
    public async Task TestRejectFriendRequest()
    {
        LoginResult? userLoginResult = await utils.CreateUser("Conner8");
        LoginResult? friendLoginResult = await utils.CreateUser("Friend8");

        string userSessionKey = userLoginResult!.SessionKey;
        string friendSessionKey = friendLoginResult!.SessionKey;

        UserDto? friend = await userService.GetUser(friendSessionKey);
        UserDto? user = await userService.GetUser(userSessionKey);
        Assert.NotNull(user);
        Assert.NotNull(friend);
        string friendUsername = friend.Username;
        string userUsername = user.Username;

        await friendModificationService.SendFriendRequest(userSessionKey, friendUsername);
        bool rejectFriendResult = await friendModificationService.RejectFriendRequest(friendSessionKey, userUsername);
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
        LoginResult? userLoginResult = await utils.CreateUser("Conner9");
        LoginResult? friendLoginResult = await utils.CreateUser("Friend9");

        string userSessionKey = userLoginResult!.SessionKey;
        string friendSessionKey = friendLoginResult!.SessionKey;

        UserDto? friend = await userService.GetUser(friendSessionKey);
        UserDto? user = await userService.GetUser(userSessionKey);
        Assert.NotNull(user);
        Assert.NotNull(friend);
        string friendUsername = friend.Username;
        string userUsername = user.Username;
        bool rejectFriendResult = await friendModificationService.RejectFriendRequest(friendSessionKey, userUsername);

        Assert.False(rejectFriendResult);
    }
}