namespace Test.service;
using Server.service;
using Microsoft.Extensions.Configuration;

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
    public void TestAddUser(){
        service.AddUser("ConnerDeFeo","Sup");
        
        var user = service.GetUser("ConnerDeFeo");

        Assert.Equal("ConnerDeFeo",user.Username);
        Assert.Equal("Sup",user.Password);
    }
}
