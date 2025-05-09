namespace Server.service.concrete;
using Server.service;
using MongoDB.Driver;
using Server.model;
using MongoDB.Bson;


/// <summary>
/// Concrete implementations of the Habit service class allowing functionality
/// with a mongo database.
/// </summary>
/// <param name="_database"></param>
public class MongoHabitService(IMongoDatabase _database) : IHabitService{

    private readonly IMongoCollection<User> _users = _database.GetCollection<User>("Users");

    private async Task<User> GetUserBySessionKey(string sessionKey){
        var Filter = Builders<User>.Filter.Eq(u => u.SessionKey, sessionKey);
        return await _users.Find(Filter).FirstOrDefaultAsync();
    }

    public async Task<List<Habit>?> GetHabits(string sessionKey){
        User user = await GetUserBySessionKey(sessionKey);
        if(user!=null){
            return user.Habits;
        }
        return null;
    }

    public async Task<List<Habit>?> CreateHabit(string sessionKey,Habit habit){
        User user = await GetUserBySessionKey(sessionKey);
        if(user!=null){
            //generate a id mannually so they front-end can use it to render properly
            habit.Id = ObjectId.GenerateNewId().ToString();
            user.Habits.Add(habit);
            await _users.UpdateOneAsync(Builders<User>.Filter.Eq(u => u.SessionKey, sessionKey),Builders<User>.Update.Push(u=>u.Habits,habit));
            return user.Habits;
        }
        return null;
    }

}