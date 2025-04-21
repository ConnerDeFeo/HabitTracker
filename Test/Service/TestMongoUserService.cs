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
        var client = new MongoClient("mongodb://localhost:27017");
        client.DropDatabase("HabitTracker");
        var database = client.GetDatabase("HabitTracker");
        service = new MongoUserService(database);
    }

    [Fact]
    public async Task TestAddUser(){
        await service.CreateUser("ConnerDeFeo","12345678");

        User user = await service.GetUser("ConnerDeFeo");

        Assert.Equal("ConnerDeFeo",user.Username);
        Assert.True(PasswordHasher.VerifyPassword("12345678",user.Password));

        LoginResult result = await service.CreateUser("ConnerDeFeo","12345678");

        Assert.False(result.Success);
    }

    [Fact]
    public async Task TestAddUserFalse(){
        await service.CreateUser("ConnerDeFeo","12345678");

        LoginResult result = await service.CreateUser("ConnerDeFeo","12345678");

        Assert.False(result.Success);

        result = await service.CreateUser("Jack","1234567");

        Assert.False(result.Success);
    }

    [Fact]
    public async Task TestLogin(){
        await service.CreateUser("ConnerDeFeo","12345678");

        LoginResult result = await service.Login("ConnerDeFeo","12345678");
        Assert.True(result.Success);
        Assert.NotNull(result.Token);

        var user = await service.GetUser("ConnerDeFeo");
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
