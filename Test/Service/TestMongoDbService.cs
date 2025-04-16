namespace Test.service;
using Server.service;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

public class TestMongoDbService
{
    MongoDbService service;
    public TestMongoDbService(){
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "ConnectionStrings:MongoDb", "mongodb://localhost:27017" },
                { "HabitTracker", "TestDatabase" }
            })
            .Build();
        service = new MongoDbService(config);
    }

    [Fact]
    public async Task TestAddUser(){
        await service.AddUser("ConnerDeFeo","Sup");
        
        var user = service.GetUser("ConnerDeFeo");

        Assert.Equal("ConnerDeFeo",user.Username);
        Assert.Equal("Sup",user.Password);

        bool user2 = await service.AddUser("ConnerDeFeo","Sup");

        Assert.False(user2);
    }

    [Fact]
    public async Task TestAddUserFalse(){
        await service.AddUser("ConnerDeFeo","Sup");
        
        var user = service.GetUser("ConnerDeFeo");

        bool user2 = await service.AddUser("ConnerDeFeo","Sup");

        Assert.False(user2);
    }
}
