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
    public async Task TestGetUser(){
        LoginResult result = await service.CreateUser("ConnerDeFeo","12345678");

        User user = await service.GetUser(result.SessionKey);

        Assert.Equal("",user.Password); //should not be returning password

        User invalid = await service.GetUser("HEHEHEHA");

        Assert.Null(invalid);
    }

    [Fact]
    public async Task TestAddUser(){
        LoginResult result = await service.CreateUser("ConnerDeFeo","12345678");

        User user = await service.GetUser(result.SessionKey);

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
        LoginResult result = await service.CreateUser("ConnerDeFeo","12345678");
        User user = await service.GetUser(result.SessionKey);

        Assert.NotNull(user.SessionKey);

        LoginResult Result = await service.Login("ConnerDeFeo","12345678");
        Assert.True(Result.Success);
        Assert.NotNull(Result.SessionKey);

    }

    [Fact]
    public async Task TestLoginFaliure(){
        await service.CreateUser("ConnerDeFeo","12345678");

        LoginResult result = await service.Login("ConnerDeFeo","Suk");
        Assert.False(result.Success);
        Assert.Equal("",result.SessionKey);
    }
}
