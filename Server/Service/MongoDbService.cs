namespace Server.service;
using MongoDB.Driver;
using MongoDB.Bson;
using Server.model;

public class MongoDbService
{
    private readonly IMongoDatabase _database;

        public MongoDbService(IConfiguration config)
        {
            var client = new MongoClient(config.GetConnectionString("MongoDb"));
            client.DropDatabase("HabitTracker");
            _database = client.GetDatabase("HabitTracker");
        }
        public void AddUser(string username)
        {
            var collection = _database.GetCollection<BsonDocument>("Users");
            var document = new BsonDocument { { "Username", username } };
            collection.InsertOne(document);
        }

        public User GetUser(string username){
            var filter = Builders<User>.Filter.Eq(u => u.Username, username);

            return _database.GetCollection<User>("Users").Find(filter).FirstOrDefault();

        }
}