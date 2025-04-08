namespace Server.Service;
using MongoDB.Driver;
using MongoDB.Bson;

public class MongoDbService
{
    private readonly IMongoDatabase _database;

        public MongoDbService(IConfiguration config)
        {
            var client = new MongoClient(config.GetConnectionString("MongoDb"));
            _database = client.GetDatabase(config["HabitTracker"]);
        }
        public void AddUser(string username)
        {
            var collection = _database.GetCollection<BsonDocument>("Users");
            var document = new BsonDocument { { "username", username } };
            collection.InsertOne(document);
        }
}