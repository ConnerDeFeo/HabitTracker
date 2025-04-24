namespace Server.service.concrete;
using MongoDB.Driver;
using Server.model;
using System.Security.Cryptography;

public class MongoUserService(IMongoDatabase _database) : IUserService
{
    private readonly IMongoCollection<User> _users = _database.GetCollection<User>("Users");

    private async Task<User> GetUserSensitive(string username){
        var Filter = Builders<User>.Filter.Eq(u => u.Username, username);

        return await _users.Find(Filter).FirstOrDefaultAsync();
    }

    public async Task<User?> GetUser(string sessionKey){
        var Filter = Builders<User>.Filter.Eq(u => u.SessionKey, sessionKey);
        var Projection = Builders<User>.Projection.Exclude(u => u.Password);

        return await _users.Find(Filter).Project<User>(Projection).FirstOrDefaultAsync();
    }
    
    public async Task<LoginResult> CreateUser(string username, string password){

        if(username==null || username.Equals("") || password==null || password.Length<8 || await GetUserSensitive(username)!=null){
            return new LoginResult{Success = false};
        }

        byte[] key = RandomNumberGenerator.GetBytes(32);
        string sessionKey = Convert.ToBase64String(key);

        var User = new User
        {
            Username = username,
            Password = PasswordHasher.HashPassword(password),
            SessionKey=sessionKey,
        };

        await _users.InsertOneAsync(User);
        return new LoginResult{Success = true, SessionKey=sessionKey};
    }

    public async Task<LoginResult> Login(string username, string password){
        User User = await GetUserSensitive(username);
        if(User!=null && PasswordHasher.VerifyPassword(password, User.Password)){
            byte[] key = RandomNumberGenerator.GetBytes(32);
            string sessionKey = Convert.ToBase64String(key);

            await _users.UpdateOneAsync(u => u.Username.Equals(username), Builders<User>.Update.Set(u => u.SessionKey, sessionKey));
            
            return new LoginResult{Success = true, SessionKey=sessionKey};
        }
        return new LoginResult{Success = false};
    }

    public async Task<bool> Logout(string sessionKey){
        User? User = await GetUser(sessionKey);
        if(User!=null && User.SessionKey.Equals(sessionKey)){
            await _users.UpdateOneAsync(u=>u.SessionKey.Equals(sessionKey),Builders<User>.Update.Set(u=>u.SessionKey, ""));
            return true;
        }
        return false;
    }

}