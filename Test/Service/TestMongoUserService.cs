namespace Test.service;
using MongoDB.Driver;
using Server.service;
using Server.service.concrete;
using Server.model.user;
using Server.model.habit;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using MongoDB.Bson;

public class TestMongoUserService{
    IMongoDatabase database;
    IUserService userService;
    IHabitService habitService;
    public TestMongoUserService()
    {
        var client = new MongoClient("mongodb://localhost:27017");
        client.DropDatabase("TestMongoUserService");
        database = client.GetDatabase("TestMongoUserService");
        userService = new MongoUserService(database);
        habitService = new MongoHabitService(database);
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
    public async Task TestGetUser()
    {
        LoginResult result = await userService.CreateUser("ConnerDeFeo", "12345678");

        UserDto? user = await userService.GetUser(result.SessionKey);

        Assert.Equal("ConnerDeFeo", user!.Username);
    }

    [Fact]
    public async Task TestGetUserInvalid(){
        UserDto? invalid = await userService.GetUser("HEHEHEHA");

        Assert.Null(invalid);
    }

    [Fact]
    public async Task TestCreateUser()
    {
        LoginResult result = await userService.CreateUser("ConnerDeFeo", "12345678");

        string sessionKey = result.SessionKey;

        UserDto? user = await userService.GetUser(result.SessionKey);

        Assert.Equal("ConnerDeFeo", user!.Username);

        LoginResult Result = await userService.CreateUser("ConnerDeFeo", "12345678");

        Assert.False(Result.Success);

    }

    [Fact]
    public async Task TestCreateUserFalse(){
        await userService.CreateUser("ConnerDeFeo","12345678");

        LoginResult Result = await userService.CreateUser("ConnerDeFeo","12345678");

        Assert.False(Result.Success);

        Result = await userService.CreateUser("Jack","1234567");

        Assert.False(Result.Success);
    }

    [Fact]
    public async Task TestLogin()
    {
        LoginResult result = await userService.CreateUser("ConnerDeFeo", "12345678");

        LoginResult Result = await userService.Login("ConnerDeFeo", "12345678");
        Assert.True(Result.Success);
        Assert.NotNull(Result.SessionKey);

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
        LoginResult result = await userService.Login(username, password);

        HabitCollection? habitCollectionUpdated = await GetHabitCollection(result.SessionKey);

        //in case i test at the begining of a month
        int total = 0;
        foreach (var kvp in habitCollectionUpdated!.HabitHistory.Values) 
            total += 1;
        

        Assert.Equal(6, total);

    }

    [Fact]
    public async Task TestLoginFaliure(){
        await userService.CreateUser("ConnerDeFeo","12345678");

        LoginResult result = await userService.Login("ConnerDeFeo","Suk");
        Assert.False(result.Success);
    }

    [Fact]
    public async Task TestLogout(){
        LoginResult result = await userService.CreateUser("ConnerDeFeo","12345678");

        bool logedOut = await userService.Logout(result.SessionKey);

        Assert.True(logedOut);
    }

    [Fact]
    public async Task TestLogoutFaliure(){
        LoginResult result = await userService.CreateUser("ConnerDeFeo","12345678");

        bool logedIn = await userService.Logout("");

        Assert.False(logedIn);
    }
}
