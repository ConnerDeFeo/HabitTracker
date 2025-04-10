namespace Test;
using Server.service;
using Microsoft.Extensions.Configuration;

public class TestInitialization
{
    MongoDbService service;
    public TestInitialization()
    {
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
    public void TestInstantiation(){
        Assert.NotNull(service);
    }
    
}
