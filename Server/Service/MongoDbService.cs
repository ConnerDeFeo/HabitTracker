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

        public async Task<User> GetUser(string username){
            var filter = Builders<User>.Filter.Eq(u => u.Username, username);

            return await _database.GetCollection<User>("Users").Find(filter).FirstOrDefaultAsync();

        }
        public async Task<bool> CreateUser(string username, string password){

            if(username==null || password==null || await GetUser(username)!=null){
                return false;
            }

            var collection = _database.GetCollection<BsonDocument>("Users");

            await collection.InsertOneAsync(new BsonDocument
            {
                { "Username", username },
                { "Password", PasswordHasher.HashPassword(password) }
            });
            return false;
        }

        public async Task<bool> Login(string username, string password){
            User user = await GetUser(username);
            if(user!=null){
                return PasswordHasher.VerifyPassword(password, user.Password);
            }
            return false;
        }
}