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
    private readonly FilterDefinitionBuilder<User> filter = Builders<User>.Filter;
    private readonly UpdateDefinitionBuilder<User> update = Builders<User>.Update;
    private readonly FindOneAndUpdateOptions<User> options = new()
    {
        ReturnDocument = ReturnDocument.After
    };

    public async Task<List<Habit>?> GetHabits(string sessionKey){
        var findUser = filter.Eq(u => u.SessionKey, sessionKey);
        User user = await _users.Find(findUser).FirstOrDefaultAsync();
        if(user!=null){
            return user.Habits;
        }
        return null;
    }

    public async Task<List<Habit>?> CreateHabit(string sessionKey,Habit habit){
        //Generates set object key so that front end can use it
        habit.Id = ObjectId.GenerateNewId().ToString();
        var findUser = filter.Eq(u=>u.SessionKey, sessionKey);
        var createHabit = update.Push(u=>u.Habits,habit);
        User user = await _users.FindOneAndUpdateAsync(findUser,createHabit,options);
        if(user!=null){
            user.AddHabit(habit);
            return user.Habits;
        }
        return null;
    }

    public async Task<List<Habit>?> DeleteHabit(string sessionKey, Habit habit)
    {
        var findUser = filter.And(
            filter.Eq(u=>u.SessionKey, sessionKey),
            filter.ElemMatch(u => u.Habits, h => h.Id == habit.Id)
        );
        var deleteHabit = update.PullFilter(u=>u.Habits,h=>h.Id==habit.Id);
        User user = await _users.FindOneAndUpdateAsync(findUser,deleteHabit,options);
        if(user!=null){
            user.RemoveHabit(habit);
            return user.Habits;
        }
        return null;
    }

    public async Task<List<Habit>?> EditHabit(string sessionKey,  Habit habit)
    {
        var findUser = filter.And(
            filter.Eq(u=>u.SessionKey,sessionKey),
            filter.ElemMatch(u => u.Habits, h => h.Id == habit.Id)
        );
        var updateHabit = update.Set("Habits.$.Name",habit.Name);

        User user = await _users.FindOneAndUpdateAsync(findUser,updateHabit);
        if(user!=null){
            user.EditHabit(habit);
            return user.Habits;
        }
        return null;
    }

    /// <summary>
    /// Sets habit completed for a given date to true
    /// </summary>
    /// <param name="sessionKey">Sessionkey of the user</param>
    /// <param name="habit">habit that is going to be completed</param>
    /// <returns>true if habit exitst and completed, false else</returns>
    public async Task<bool> CompleteHabit(string sessionKey, Habit habit){
        string date = DateTime.Today.ToString("yyyy-MM-dd");

        var findUser = filter.And(
            filter.Eq(u=>u.SessionKey, sessionKey),
            filter.ElemMatch(u => u.Habits, h => h.Id == habit.Id)
        );
        var updateCompletedHabits = update.Set($"CompletedHabitsByDate.{date}.{habit.Id}",true);

        User user = await _users.FindOneAndUpdateAsync(findUser, updateCompletedHabits,options);
        return user != null && 
           user.HabitsCompletedByDate.TryGetValue(date, out var habitsForDate) && 
           habitsForDate[habit.Id!];
        }

}