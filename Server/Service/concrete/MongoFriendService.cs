namespace Server.service.concrete;

using MongoDB.Driver;
using Server.model.habit;
using Server.model.user;
using Server.service.interfaces;

// public class MongoFriendService(IMongoDatabase _database) : IFriendService
// {

//     private readonly IMongoCollection<User> _users = _database.GetCollection<User>("Users");
//     private readonly IMongoCollection<HabitCollection> _habitCollections = _database.GetCollection<HabitCollection>("HabitCollection");

//     public async Task<List<UserDto>> GetFriends(string sessionKey)
//     {
//     }

//     public Task<ProfileHabits> GetFriendProfile(string sessionKey)
//     { 
        
//     }
// }