namespace Server.service.concrete;
using Server.service;
using MongoDB.Driver;
using Server.model;


/// <summary>
/// Concrete implementations of the Habit service class allowing functionality
/// with a mongo database.
/// </summary>
/// <param name="_database"></param>
public class MongoHabitService(IMongoDatabase _database) : IHabitService{

    private readonly IMongoCollection<User> _users = _database.GetCollection<User>("Users");

    public async Task<List<Habit>?> GetHabits(string sessionKey){
        var Filter = Builders<User>.Filter.Eq(u => u.SessionKey, sessionKey);
        User user = await _users.Find(Filter).FirstOrDefaultAsync();
        if(user!=null){
            return user.Habits;
        }
        return null;
    }

    public async Task<Habit?> CreateHabit(string sessionKey,string habitName){
        User user = await _users.Find(u=>u.SessionKey == sessionKey).FirstOrDefaultAsync();
        if(user!=null){
            Habit habit = new() {Name=habitName};
            user.Habits.Add(habit);
            return habit;
        }
        return null;
    }

}