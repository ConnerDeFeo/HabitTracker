namespace Test.service;
using MongoDB.Driver;
using Server.service;
using Server.service.concrete;
using Server.model.user;
using Server.model.habit;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using MongoDB.Bson;

public class TestMongoHabitHistory
{
    string monthKey;
    string dayKey;
    IMongoDatabase database;
    IUserService userService;
    IHabitService habitService;
    IHabitHistoryService habitHistoryService;
    IHabitStatisticService habitStatisticService;
    HashSet<string> daysOfWeek;

    public TestMongoHabitHistory()
    {
        var Client = new MongoClient("mongodb://localhost:27017");
        Client.DropDatabase("TestMongoHabitHistoryService");
        database = Client.GetDatabase("TestMongoHabitHistoryService");
        userService = new MongoUserService(database);
        habitService = new MongoHabitService(database);
        habitHistoryService = new MongoHabitHistoryService(database);
        habitStatisticService = new MongoHabitStatisticService(database);
        monthKey = DateTime.Today.ToString("yyyy-MM");
        dayKey = DateTime.Today.ToString("dd");
        daysOfWeek = ["Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"];
    }

    //This ones gonna be painful
    [Fact]
    public async Task TestGetHistoricalData()
    {
        DateTime today = DateTime.Today;

        IMongoCollection<User> users = database.GetCollection<User>("Users");
        IMongoCollection<HabitCollection> collection = database.GetCollection<HabitCollection>("HabitCollection");

        string id = ObjectId.GenerateNewId().ToString();
        string past = DateTime.Today.AddDays(-15).ToString("yyyy-MM-dd");
        string password = "asdfasdf";
        string username = "Jack";

        User user = new()
        {
            Id = id,
            Username = username,
            //Hash the password before storing in database
            Password = PasswordHasher.HashPassword(password),
            LastLoginDate = past
        };

        HabitCollection habitCollection = new()
        {
            Id = id,
            ActiveHabits = [new Habit {
                Id = ObjectId.GenerateNewId().ToString(),
                Name = "Read 25 Pages",
                DateCreated = past,
                DaysActive = ["Sunday","Monday","Tuesday","Wednesday","Thursday","Friday","Saturday"],
                Type = HabitType.Numeric,
                Value = 25,
                ValueUnitType = "Pages"
            }],
            HabitHistory = [],
            NonActiveHabits = []
        };

        await users.InsertOneAsync(user);
        await collection.InsertOneAsync(habitCollection);
        LoginResult result = await userService.Login(username, password);

    }
}