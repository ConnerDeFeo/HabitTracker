namespace Server.service.concrete;
using MongoDB.Driver;
using Server.model;
using System.Security.Cryptography;

public class MongoUserService(IMongoDatabase _database) : IUserService
{
    private readonly IMongoCollection<User> _users = _database.GetCollection<User>("Users");

    private async Task<User> GetUser(string username){
        var Filter = Builders<User>.Filter.Eq(u => u.Username, username);

        return await _users.Find(Filter).FirstOrDefaultAsync();
    }

    public async Task<User> GetUserPublic(string username){
        var Filter = Builders<User>.Filter.Eq(u => u.Username, username);
        var Projection = Builders<User>.Projection.Exclude(u => u.Password);

        return await _users.Find(Filter).Project<User>(Projection).FirstOrDefaultAsync();
    }
    
    public async Task<LoginResult> CreateUser(string username, string password){

        if(username==null || username=="" || password==null || password.Length<8 || await GetUser(username)!=null){
            return new LoginResult{Success = false};
        }

        byte[] key = RandomNumberGenerator.GetBytes(32);
        string token = Convert.ToBase64String(key);

        var user = new User
        {
            Username = username,
            Password = PasswordHasher.HashPassword(password),
            SessionKey=token,
        
        };

        await _users.InsertOneAsync(user);
        return new LoginResult{Success = true, Token=token};
    }

    public async Task<LoginResult> Login(string username, string password){
        User user = await GetUser(username);
        if(user!=null && PasswordHasher.VerifyPassword(password, user.Password)){
            byte[] key = RandomNumberGenerator.GetBytes(32);
            string token = Convert.ToBase64String(key);

            await _users.UpdateOneAsync(u => u.Username == username, Builders<User>.Update.Set(u => u.SessionKey, token));
            
            return new LoginResult{Success = true, Token=token};
        }
        return new LoginResult{Success = false};
    }
}