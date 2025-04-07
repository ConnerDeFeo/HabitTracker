using Mongo2Go;
using MongoDB.Driver;
using Xunit;

public class MongoDbServiceTests
{
    private MongoDbRunner _mongoDbRunner;

    public MongoDbServiceTests()
    {
        _mongoDbRunner = MongoDbRunner.Start();
    }

    [Fact]
    public void TestDatabaseCreation()
    {

        var mongoService = new MongoDbService(_mongoDbRunner.ConnectionString, "TestDb");
        var collection = mongoService.GetCollection<BsonDocument>("TestCollection");

        // Perform operations on the collection (insert, find, etc.)
        collection.InsertOne(new BsonDocument { { "Name", "Test" } });
        var result = collection.Find(new BsonDocument()).FirstOrDefault();

        Assert.NotNull(result);
        Assert.Equal("Test", result["Name"].AsString);
    }

    public void Dispose()
    {
        _mongoDbRunner.Dispose();
    }
}
