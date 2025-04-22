namespace Test.service;
using MongoDB.Driver;
using Server.service;
using Server.service.concrete;
using Server.model;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

public class TestMongoUserService{
    IUserService service;
    public TestMongoUserService(){
        var Client = new MongoClient("mongodb://localhost:27017");
        Client.DropDatabase("HabitTracker");
        var database = Client.GetDatabase("HabitTracker");
        service = new MongoUserService(database);
    }

    [Fact]
    public async Task TestGetUserPublic(){
        await service.CreateUser("ConnerDeFeo","12345678");

        User user = await service.GetUserPublic("ConnerDeFeo");

        Assert.Equal("",user.Password); //should not be returning password

        User invalid = await service.GetUserPublic("HEHEHEHA");

        Assert.Null(invalid);
    }

    [Fact]
    public async Task TestAddUser(){
        await service.CreateUser("ConnerDeFeo","12345678");

        User user = await service.GetUserPublic("ConnerDeFeo");

        Assert.Equal("ConnerDeFeo",user.Username);

        LoginResult Result = await service.CreateUser("ConnerDeFeo","12345678");

        Assert.False(Result.Success);
    }

    [Fact]
    public async Task TestAddUserFalse(){
        await service.CreateUser("ConnerDeFeo","12345678");

        LoginResult Result = await service.CreateUser("ConnerDeFeo","12345678");

        Assert.False(Result.Success);

        Result = await service.CreateUser("Jack","1234567");

        Assert.False(Result.Success);
    }

    [Fact]
    public async Task TestLogin(){
        await service.CreateUser("ConnerDeFeo","12345678");

        LoginResult Result = await service.Login("ConnerDeFeo","12345678");
        Assert.True(Result.Success);
        Assert.NotNull(Result.Token);

        var user = await service.GetUserPublic("ConnerDeFeo");
        Assert.NotNull(user.SessionKey);
    }

    [Fact]
    public async Task TestLoginFaliure(){
        await service.CreateUser("ConnerDeFeo","12345678");

        LoginResult result = await service.Login("ConnerDeFeo","Suk");
        Assert.False(result.Success);
        Assert.Null(result.Token);
    }
}
