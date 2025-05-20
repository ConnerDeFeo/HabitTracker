namespace Server.service.concrete;
using MongoDB.Driver;
using Server.model;
using System.Security.Cryptography;


/// <summary>
/// Concrete implementations of the User service class allowing functionality
/// with a mongo database.
/// </summary>
/// <param name="_database"></param>
public class MongoUserService(IMongoDatabase _database) : IUserService
{
    private readonly IMongoCollection<User> _users = _database.GetCollection<User>("Users");
    private readonly UpdateDefinitionBuilder<User> update = Builders<User>.Update;
    
    private async Task<User> GetUserByUsername(string username = "")
    {
        var Filter = Builders<User>.Filter.Eq(u => u.Username, username);

        return await _users.Find(Filter).FirstOrDefaultAsync();
    }

    private async Task<User> GetUserBySessionKey(string sessionKey=""){
        var Filter = Builders<User>.Filter.Eq(u => u.SessionKey, sessionKey);

        return await _users.Find(Filter).FirstOrDefaultAsync();
    }

    //This is the public version of getUser, not exposing any sensitive info
    public async Task<UserDto?> GetUser(string sessionKey){

        User user = await GetUserBySessionKey(sessionKey);
        if(user!=null) 
            return new UserDto{
                Username = user.Username,
            };
        return null;
    }
    
    /// <summary>
    /// Creates a new username should the username not already exist.
    /// Generates a random sessionKey for the User to user immediately
    /// </summary>
    /// <returns>Login result containing sessionKey if succesful</returns>
    public async Task<LoginResult> CreateUser(string username, string password){

        //username and password valid, User does not exists, password long enough 
        if(username==null || username.Equals("") || password==null || password.Length<8 || await GetUserByUsername(username)!=null){
            return new LoginResult{Success = false};
        }

        //Generate random sessionKey
        byte[] key = RandomNumberGenerator.GetBytes(32);
        string sessionKey = Convert.ToBase64String(key);

        var User = new User
        {
            Username = username,
            //Hash the password before storing in database
            Password = PasswordHasher.HashPassword(password),
            SessionKey=sessionKey,
        };

        await _users.InsertOneAsync(User);
        return new LoginResult{Success = true, SessionKey=sessionKey};
    }

    /// <summary>
    /// Logs in user if password and username are valid.
    /// Uses PasswordHasher class for password decryption
    /// </summary>
    /// <returns>LoginRefult containing sessionKey if succsesful</returns>
    public async Task<LoginResult> Login(string username, string password){
        User user = await GetUserByUsername(username);

        if (user != null && PasswordHasher.VerifyPassword(password, user.Password))
        {
            byte[] key = RandomNumberGenerator.GetBytes(32);
            string sessionKey = Convert.ToBase64String(key);

            await _users.UpdateOneAsync(u => u.Username.Equals(username), update.Set(u => u.SessionKey, sessionKey));

            return new LoginResult { Success = true, SessionKey = sessionKey };
        }
        return new LoginResult{Success = false};
    }

    public async Task<bool> Logout(string sessionKey){
        User user = await GetUserBySessionKey(sessionKey);
        if(user!=null && user.SessionKey.Equals(sessionKey)){
            await _users.UpdateOneAsync(u=>u.SessionKey.Equals(sessionKey),update.Set(u=>u.SessionKey, ""));
            return true;
        }
        return false;
    }


}