namespace Server.Services;
using MongoDB.Driver;

    public class MongoDbService
    {
        private readonly IMongoDatabase _database;

        public MongoDbService(IConfiguration config)
        {
            var client = new MongoClient(config.GetConnectionString("MongoDb"));
            _database = client.GetDatabase(config["HabitTracker"]);
        }
    }