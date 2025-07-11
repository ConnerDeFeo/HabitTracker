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
    private readonly string thisMonth = DateTime.Today.ToString("yyyy-MM");
    private readonly string thisDay = DateTime.Today.ToString("dd");

    //Generates random sessionKey
    private static string GenerateSessionKey()
    {
        byte[] key = RandomNumberGenerator.GetBytes(32);
        return Convert.ToBase64String(key);
    }

    //Updates users habit history to keep up with current dates
    private async Task UpdateUserHistory(User user)
    {
        //Get habit collection for updating missing dates
        var filter = Builders<HabitCollection>.Filter.Eq(hc => hc.Id, user.Id);
        HabitCollection collection = await _habitCollections
            .Find(filter)
            .Project<HabitCollection>(Builders<HabitCollection>.Projection.Include(hc => hc.ActiveHabits))
            .FirstOrDefaultAsync();
        DateTime today = DateTime.Today.Date;
        List<UpdateDefinition<HabitCollection>> habitHistoryUpdates = HabitUtils.UpdateUserHabitHistory(user, collection, today);
        if(habitHistoryUpdates.Count > 0)
            await _habitCollections.UpdateOneAsync(filter, Builders<HabitCollection>.Update.Combine(habitHistoryUpdates));
    }

    //This is the public version of getUser, not exposing any sensitive info
    public async Task<UserDto?> GetUser(string sessionKey)
    {

        User? user = await UserUtils.GetUserBySessionKey(sessionKey, _users);
        if (user != null)
        {
            await UpdateUserHistory(user);
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
    public async Task<LoginResult> CreateUser(LoginRequest request){

        string username = request.Username;
        string password = request.Password;
        //username and password valid, User does not exists, password long enough 
        if (username is null || username.Equals("") || password is null || password.Length < 8 || await UserUtils.GetUserByUsername(username, _users) is not null) {
            return new LoginResult { SessionKey = "" };
        }

        string sessionKey = GenerateSessionKey();
        string id = ObjectId.GenerateNewId().ToString();
        Dictionary<string, string> sessionKeys = [];
        sessionKeys[sessionKey] = request.DeviceId;
        string today = $"{thisMonth}-{thisDay}";
        User user = new()
        {
            Id = id,
            Username = username,
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
        return new LoginResult { SessionKey = sessionKey, User=new UserDto { Username = username, DateCreated = today, Id = id } };
    }

    /// <summary>
    /// Logs in user if password and username are valid.
    /// Uses PasswordHasher class for password decryption
    /// </summary>
    /// <returns>LoginRefult containing sessionKey if succsesful</returns>
    public async Task<LoginResult> Login(LoginRequest request){
        User? user = await UserUtils.GetUserByUsername(request.Username, _users);

        if (user is not null && PasswordHasher.VerifyPassword(request.Password, user.Password))
        {
            string username = request.Username;
            await UpdateUserHistory(user);
            
            string sessionKey = GenerateSessionKey();
            await _users.UpdateOneAsync(
                u => u.Username.Equals(username),
                BuilderUtils.userUpdate.Combine(
                    BuilderUtils.userUpdate.Set($"SessionKeys.{sessionKey}", request.DeviceId),
                    BuilderUtils.userUpdate.Set(u => u.LastLoginDate, DateTime.Today.ToString("yyyy-MM-dd"))
                )
            );

            return new LoginResult
            {
                SessionKey = sessionKey,
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
        return new LoginResult{SessionKey = ""};
    }

    public async Task<bool> Logout(string sessionKey){
        User? user = await UserUtils.GetUserBySessionKey(sessionKey,_users);
        if(user is not null){
            await _users.UpdateOneAsync(u=>u.Id!.Equals(user.Id),BuilderUtils.userUpdate.Unset($"SessionKeys.{sessionKey}"));
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
                return UserUtils.GetProfile(collection,user);
        }
        return null;
    }

    /// <summary>
    /// Indexes session keys to make lookup faster, only used on server startup
    /// </summary>
    public void CreateSessionKeyIndexes()
    {
        var indexKeys = Builders<User>.IndexKeys.Ascending(u => u.SessionKeys);
        var indexModel = new CreateIndexModel<User>(indexKeys);
        _users.Indexes.CreateOne(indexModel); // Run once on startup
    }
}