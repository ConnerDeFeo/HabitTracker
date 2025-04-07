using MongoDB.Driver;

    public class MongoDbService
    {
        private readonly IMongoDatabase _database;

        public MongoDbService(IConfiguration config)
        {
            var client = new MongoClient(config.GetConnectionString("MongoDb"));
            _database = client.GetDatabase(config["HabitTracker"]);
        }

        public IMongoCollection<T> GetCollection<T>(string name)
        {
            return _database.GetCollection<T>(name);
        }

        public void InsertDocument<T>(string collectionName, T document)
        {
            var collection = GetCollection<T>(collectionName);
            collection.InsertOne(document); 
        }

        // Optionally, you can also add a method to insert multiple documents
        public void InsertDocuments<T>(string collectionName, IEnumerable<T> documents)
        {
            var collection = GetCollection<T>(collectionName);
            collection.InsertMany(documents);
        }
    }