namespace Server.service.concrete;
using MongoDB.Driver;
using Server.model;
using System.Security.Cryptography;

public class MongoUserService(IMongoDatabase _database) : IUserService
{
    private readonly IMongoCollection<User> _users = _database.GetCollection<User>("Users");

    private async Task<User> GetUserSensitive(string Username){
        var Filter = Builders<User>.Filter.Eq(u => u.Username, Username);

        return await _users.Find(Filter).FirstOrDefaultAsync();
    }

    public async Task<User> GetUser(string SessionKey){
        var Filter = Builders<User>.Filter.Eq(u => u.SessionKey, SessionKey);
        var Projection = Builders<User>.Projection.Exclude(u => u.Password);

        return await _users.Find(Filter).Project<User>(Projection).FirstOrDefaultAsync();
    }
    
    public async Task<LoginResult> CreateUser(string Username, string Password){

        if(Username==null || Username=="" || Password==null || Password.Length<8 || await GetUserSensitive(Username)!=null){
            return new LoginResult{Success = false};
        }

        byte[] key = RandomNumberGenerator.GetBytes(32);
        string SessionKey = Convert.ToBase64String(key);

        var user = new User
        {
            Username = Username,
            Password = PasswordHasher.HashPassword(Password),
            SessionKey=SessionKey,
        
        };

        await _users.InsertOneAsync(user);
        return new LoginResult{Success = true, SessionKey=SessionKey};
    }

    public async Task<LoginResult> Login(string Username, string Password){
        User user = await GetUserSensitive(Username);
        if(user!=null && PasswordHasher.VerifyPassword(Password, user.Password)){
            byte[] key = RandomNumberGenerator.GetBytes(32);
            string SessionKey = Convert.ToBase64String(key);

            await _users.UpdateOneAsync(u => u.Username == Username, Builders<User>.Update.Set(u => u.SessionKey, SessionKey));
            
            return new LoginResult{Success = true, SessionKey=SessionKey};
        }
        return new LoginResult{Success = false};
    }
}