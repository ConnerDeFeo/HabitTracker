namespace Test.service;
using MongoDB.Driver;
using Server.service;
using Server.model;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

public class TestUserService{
    UserService service;
    public TestUserService(){
        var client = new MongoClient("mongodb://localhost:27017");
        client.DropDatabase("HabitTracker");
        var database = client.GetDatabase("HabitTracker");
        service = new UserService(database);
    }

    [Fact]
    public async Task TestAddUser(){
        await service.CreateUser("ConnerDeFeo","Sup");

        var user = await service.GetUser("ConnerDeFeo");

        Assert.Equal("ConnerDeFeo",user.Username);
        Assert.True(PasswordHasher.VerifyPassword("Sup",user.Password));

        bool user2 = await service.CreateUser("ConnerDeFeo","Sup");

        Assert.False(user2);
    }

    [Fact]
    public async Task TestAddUserFalse(){
        await service.CreateUser("ConnerDeFeo","Sup");

        bool user2 = await service.CreateUser("ConnerDeFeo","Sup");

        Assert.False(user2);
    }

    [Fact]
    public async Task TestLogin(){
        await service.CreateUser("ConnerDeFeo","Sup");

        LoginResult result = await service.Login("ConnerDeFeo","Sup");
        Assert.True(result.Success);
        Assert.NotNull(result.Token);

        var user = await service.GetUser("ConnerDeFeo");
        Assert.NotNull(user.SessionKey);
    }

    [Fact]
    public async Task TestLoginFaliure(){
        await service.CreateUser("ConnerDeFeo","Sup");

        LoginResult result = await service.Login("ConnerDeFeo","Suk");
        Assert.False(result.Success);
        Assert.Null(result.Token);
    }
}
