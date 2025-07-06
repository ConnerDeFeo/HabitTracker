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
    HashSet<string> daysOfWeek;

    public TestMongoFriendService()
    {
        var Client = new MongoClient("mongodb://localhost:27017");
        dbName = $"TestMongoFriendService_{Guid.NewGuid().ToString()[..20]}";
        database = Client.GetDatabase(dbName);
        userService = new MongoUserService(database);
        habitService = new MongoHabitService(database);
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

    public async Task TestSendFriendRequest()
    {
        UserDto? user = await userService.CreateUser("Conner", "12341234");
        UserDto? friend = await userService.CreateUser("Friend", "12341234");

        string userSessionKey = user?.SessionKey;
        string friendUserName = friend?.username;
        bool addFriendResult = await userService.AddFriend(userSessionKey, friendUserName);

        Assert.True(addFriendResult);

        UserDto? postUserDto = await userService.GetUser(userSessionKey);
        Assert.NotNull(postUserDto);
        Assert.Empty(postUserDto.Friends);
        Assert.Empty(postUserDto.FriendRequests);
        Assert.Single(postUserDto.FriendRequestsSent);

        UserDto? postFriendDto = await userService.GetUser(friend?.SessionKey);
        Assert.NotNull(postFriendDto);
        Assert.Single(postFriendDto.FriendRequests);
        Assert.Empty(postFriendDto.Friends);
        Assert.Empty(postFriendDto.FriendRequestsSent);

        bool addFriendResult = await userService.AddFriend(userSessionKey, friendUserName);
        Assert.True(addFriendResult);

        postUserDto = await userService.GetUser(userSessionKey);
        Assert.NotNull(postUserDto);
        Assert.Single(postUserDto?.Friends);
        Assert.Empty(postUserDto?.FriendRequests);
        Assert.Empty(postFriendDto.FriendRequestsSent);

        postFriendDto = await userService.GetUser(friend?.SessionKey);
        Assert.NotNull(postFriendDto);
        Assert.Single(postFriendDto?.Friends);
        Assert.Empty(postFriendDto.FriendRequestsSent);

    }
    public async Task TestSendFriendRequestFaliure()
    {
        UserDto? user = await userService.CreateUser("Conner2", "12341234");
        UserDto? friend = await userService.CreateUser("Friend2", "12341234");

        string userSessionKey = user?.SessionKey;
        string friendUserName = friend?.username;
        bool addFriendResult = await userService.AddFriend(userSessionKey, friendUserName);

        addFriendResult = await userService.AddFriend(userSessionKey, friendUserName);
        Assert.False(addFriendResult);

        UserDto? postUserDto = await userService.GetUser(userSessionKey);
        Assert.NotNull(postUserDto);
        Assert.Empty(postUserDto.Friends);
        Assert.Empty(postUserDto.FriendRequests);
        Assert.Single(postUserDto.FriendRequestsSent);

        UserDto? postFriendDto = await userService.GetUser(friend?.SessionKey);
        Assert.NotNull(postFriendDto);
        Assert.Single(postFriendDto.FriendRequests);
        Assert.Empty(postFriendDto.Friends);
        Assert.Empty(postFriendDto.FriendRequestsSent);

    }

    public async Task TestUnSendFriendRequest()
    {
        User? user = await userService.CreateUser("Conner3", "12341234");
        User? friend = await userService.CreateUser("Friend3", "12341234");

        string userSessionKey = user?.SessionKey;
        string friendUserName = friend?.username;
        bool addFriendResult = await userService.AddFriend(userSessionKey, friendUserName);

        Assert.True(addFriendResult);

        bool unSendFriendResult = await userService.UnSendFriendRequest(userSessionKey, friendUserName);

        Assert.True(unSendFriendResult);

        UserDto? postUserDto = await userService.GetUser(userSessionKey);
        Assert.NotNull(postUserDto);
        Assert.Empty(postUserDto.Friends);
        Assert.Empty(postUserDto.FriendRequests);
        Assert.Empty(postUserDto.FriendRequestsSent);

        UserDto? postFriendDto = await userService.GetUser(friend?.SessionKey);
        Assert.NotNull(postFriendDto);
        Assert.Empty(postFriendDto.FriendRequests);
        Assert.Empty(postFriendDto.Friends);
        Assert.Empty(postFriendDto.FriendRequestsSent);

    }
    public async Task TestUnSendFriendRequestFaliure()
    {
        User? user = await userService.CreateUser("Conner4", "12341234");
        User? friend = await userService.CreateUser("Friend4", "12341234");

        string userSessionKey = user?.SessionKey;
        string friendUserName = friend?.username;

        bool unSendFriendResult = await userService.UnSendFriendRequest(userSessionKey, friendUserName);

        Assert.True(unSendFriendResult);

        UserDto? postUserDto = await userService.GetUser(userSessionKey);
        Assert.NotNull(postUserDto);
        Assert.Empty(postUserDto.Friends);
        Assert.Empty(postUserDto.FriendRequests);
        Assert.Single(postUserDto.FriendRequestsSent);

        UserDto? postFriendDto = await userService.GetUser(friend?.SessionKey);
        Assert.NotNull(postFriendDto);
        Assert.Single(postFriendDto.FriendRequests);
        Assert.Empty(postFriendDto.Friends);
        Assert.Empty(postFriendDto.FriendRequestsSent);

    }
    

}