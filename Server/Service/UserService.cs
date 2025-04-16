namespace Server.service;
using MongoDB.Driver;
using MongoDB.Bson;
using Server.model;
using System.Security.Cryptography;

public class UserService(IMongoDatabase _database)
{
    private readonly IMongoCollection<User> _users = _database.GetCollection<User>("Users");

    public async Task<User> GetUser(string username){
            var filter = Builders<User>.Filter.Eq(u => u.Username, username);

            return await _users.Find(filter).FirstOrDefaultAsync();

        }
    public async Task<bool> CreateUser(string username, string password){

        if(username==null || password==null || await GetUser(username)!=null){
            return false;
        }

        var user = new User
        {
            Username = username,
            Password = PasswordHasher.HashPassword(password)
        };

        await _users.InsertOneAsync(user);
        return true;
    }

    public async Task<LoginResult> Login(string username, string password){
        User user = await GetUser(username);
        if(user!=null && PasswordHasher.VerifyPassword(password, user.Password)){
            byte[] key = RandomNumberGenerator.GetBytes(32);
            return new LoginResult{Success = true, Token=Convert.ToBase64String(key)};
        }
        return new LoginResult{Success = false};
    }
}