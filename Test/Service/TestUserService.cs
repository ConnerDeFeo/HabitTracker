namespace Test.service;
using MongoDB.Driver;
using Server.service;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

public class TestUserService{
    UserService service;
    public TestUserService(){
        var client = new MongoClient("mongodb://localhost:27017");
        var database = client.GetDatabase("TestDatabase");
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
        
        var user = await service.GetUser("ConnerDeFeo");

        bool user2 = await service.CreateUser("ConnerDeFeo","Sup");

        Assert.False(user2);
    }
}
