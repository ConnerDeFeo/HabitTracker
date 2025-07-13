namespace Server.service.concrete;
using MongoDB.Driver;
using Server.model.user;
using Server.model.habit;
using System.Security.Cryptography;
using MongoDB.Bson;
using Server.service.interfaces;
using Server.service.utils;
using Server.dtos;

/// <summary>
/// Concrete implementations of the User service class allowing functionality
/// with a mongo database.
/// </summary>
public class MongoUserService(IMongoDatabase _database) : IUserService
{
    private readonly IMongoCollection<User> _users = _database.GetCollection<User>("Users");
    private readonly IMongoCollection<HabitCollection> _habitCollections = _database.GetCollection<HabitCollection>("HabitCollection");
    private readonly string thisMonth = DateTime.UtcNow.ToString("yyyy-MM");
    private readonly string thisDay = DateTime.UtcNow.ToString("dd");

    //This is the public version of getUser, not exposing any sensitive info
    public async Task<UserDto?> GetUser(string sessionKey)
    {

        User? user = await UserUtils.GetUserBySessionKey(sessionKey, _users);
        if (user != null)
        {
            await UserUtils.UpdateUserHistory(user, _habitCollections);
            return new UserDto
            {
                Username = user.Username,
                DateCreated = user.DateCreated,
                Id = user.Id!,
                FriendRequests = user.FriendRequests,
                Friends = user.Friends,
                FriendRequestsSent = user.FriendRequestsSent
            };
        }
        return null;
    }

    /// <summary>
    /// Creates a new username should the username not already exist.
    /// Generates a random sessionKey for the User to user immediately
    /// </summary>
    /// <returns>Login result containing sessionKey if succesful</returns>
    public async Task<LoginResult> CreateUser(LoginRequest request)
    {

        string username = request.Username;
        string password = request.Password;
        string? email = request.Email;
        //username and password valid, User does not exists, password long enough 
        if (string.IsNullOrEmpty(username) || password is null || password.Length < 8 || email is null)
        {
            return new LoginResult { SessionKey = "" };
        }
        //user should not already exist in terms of email or username
        var Filter = BuilderUtils.userFilter.Or(
            BuilderUtils.userFilter.Eq(u => u.Username, username),
            BuilderUtils.userFilter.Eq(u => u.Email, email)
        );
        User? exists = await _users.Find(Filter).FirstOrDefaultAsync();
        if (exists is not null)
            return new LoginResult { SessionKey = "" };

        string sessionKey = UserUtils.GenerateSessionKey();
        string id = ObjectId.GenerateNewId().ToString();
        Dictionary<string, string> sessionKeys = [];
        sessionKeys[sessionKey] = request.DeviceId;
        string today = $"{thisMonth}-{thisDay}";
        User user = new()
        {
            Id = id,
            Username = username,
            Email = email,
            //Hash the password before storing in database
            Password = PasswordHasher.HashPassword(password),
            SessionKeys = sessionKeys,
            LastLoginDate = today,
            DateCreated = today
        };

        await _users.InsertOneAsync(user);
        HabitCollection collection = new() { Id = id };
        collection.HabitHistory[thisMonth] = [];
        collection.HabitHistory[thisMonth][thisDay] = new();
        await _habitCollections.InsertOneAsync(collection);
        return new LoginResult { SessionKey = sessionKey, User = new UserDto { Username = username, DateCreated = today, Id = id } };
    }

    /// <summary>
    /// Logs in user if password and username are valid.
    /// Uses PasswordHasher class for password decryption
    /// </summary>
    /// <returns>LoginRefult containing sessionKey if succsesful</returns>
    public async Task<LoginResult> Login(LoginRequest request)
    {
        User? user = await UserUtils.GetUserByUsername(request.Username, _users);

        if (user is not null && PasswordHasher.VerifyPassword(request.Password, user.Password))
        {
            string username = request.Username;
            await UserUtils.UpdateUserHistory(user, _habitCollections);

            //New sessionKey for device
            string newSessionKey = await UserUtils.UpdateSessionKeys(user, request.DeviceId, _users);

            return new LoginResult
            {
                SessionKey = newSessionKey,
                User =
                new UserDto
                {
                    Username = username,
                    DateCreated = user.DateCreated,
                    Id = user.Id!,
                    FriendRequests = user.FriendRequests,
                    Friends = user.Friends,
                    FriendRequestsSent = user.FriendRequestsSent
                }
            };
        }
        return new LoginResult { SessionKey = "" };
    }

    public async Task<bool> Logout(string sessionKey)
    {
        User? user = await UserUtils.GetUserBySessionKey(sessionKey, _users);
        if (user is not null)
        {
            await _users.UpdateOneAsync(u => u.Id!.Equals(user.Id), BuilderUtils.userUpdate.Unset($"SessionKeys.{sessionKey}"));
            return true;
        }
        return false;
    }


    public async Task<Profile?> GetProfile(string sessionKey)
    {
        User? user = await UserUtils.GetUserBySessionKey(sessionKey, _users);
        if (user is not null)
        {
            string userId = user.Id!;
            HabitCollection? collection = await _habitCollections
                .Find(hc => hc.Id!.Equals(userId))
                .FirstOrDefaultAsync();

            if (collection is not null)
                return UserUtils.GetProfile(collection, user);
        }
        return null;
    }

    public async Task<bool> ChangeUsername(string sessionKey, string newUsername)
    { 
        User? user = await UserUtils.GetUserBySessionKey(sessionKey, _users);
        if (user is not null)
        {
            User? exists = await _users.Find(u => u.Username == newUsername).FirstOrDefaultAsync();
            if (exists is not null)
                return false;
            await _users.UpdateOneAsync(
                u => u.Username == user.Username,
                BuilderUtils.userUpdate.Set(u => u.Username, newUsername)
            );
            return true;
        }
        return false;
    } 
}