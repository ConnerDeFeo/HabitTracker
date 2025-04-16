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
        public async Task<bool> AddUser(string username, string password){

            if(username==null || password==null){
                return false;
            }

            var collection = _database.GetCollection<BsonDocument>("Users");

            //if user exists, return false
            if(await collection.Find(Builders<BsonDocument>.Filter.Eq("Username", username)).FirstOrDefaultAsync()!=null){
                return false;
            }

            await collection.InsertOneAsync(new BsonDocument
            {
                { "Username", username },
                { "Password", PasswordHasher.HashPassword(password) }
            });
            return true;
        }

        public User GetUser(string username){
            var filter = Builders<User>.Filter.Eq(u => u.Username, username);

            return _database.GetCollection<User>("Users").Find(filter).FirstOrDefault();

        }
}