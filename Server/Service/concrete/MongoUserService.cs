namespace Server.service.concrete;
using MongoDB.Driver;
using Server.model.user;
using Server.model.habit;
using System.Security.Cryptography;
using MongoDB.Bson;

/// <summary>
/// Concrete implementations of the User service class allowing functionality
/// with a mongo database.
/// </summary>
/// <param name="_database"></param>
public class MongoUserService(IMongoDatabase _database) : IUserService
{
    private readonly IMongoCollection<User> _users = _database.GetCollection<User>("Users");
    private readonly IMongoCollection<HabitCollection> _habitCollections = _database.GetCollection<HabitCollection>("HabitCollection");
    private readonly UpdateDefinitionBuilder<User> update = Builders<User>.Update;


    private static string GenerateSessionKey()
    { 
        byte[] key = RandomNumberGenerator.GetBytes(32);
        return Convert.ToBase64String(key);
    }
    private async Task<User> GetUserByUsername(string username)
    {
        var Filter = Builders<User>.Filter.Eq(u => u.Username, username);

        return await _users.Find(Filter).FirstOrDefaultAsync();
    }

    private async Task<User> GetUserBySessionKey(string sessionKey){
        var Filter = Builders<User>.Filter.Eq(u => u.SessionKey, sessionKey);

        return await _users.Find(Filter).FirstOrDefaultAsync();
    }

    //This is the public version of getUser, not exposing any sensitive info
    public async Task<UserDto?> GetUser(string sessionKey)
    {

        User user = await GetUserBySessionKey(sessionKey);
        if (user != null)
            return new UserDto
            {
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
        if(username is null || username.Equals("") || password is null || password.Length<8 || await GetUserByUsername(username)is not null){
            return new LoginResult{Success = false};
        }

        string sessionKey = GenerateSessionKey();
        string today = DateTime.Today.ToString("yyyy-MM-dd");
        User user = new()
        {
            Username = username,
            //Hash the password before storing in database
            Password = PasswordHasher.HashPassword(password),
            SessionKey = sessionKey,
            LastLoginDate = today
        };

        string id = ObjectId.GenerateNewId().ToString();
        user.Id=id;
        string thisMonth = DateTime.Today.ToString("yyyy-MM");
        string thisDay = DateTime.Today.ToString("dd");

        await _users.InsertOneAsync(user);
        HabitCollection collection = new() { Id = id };
        collection.HabitHistory[thisMonth] = [];
        collection.HabitHistory[thisMonth][thisDay] = new();
        await _habitCollections.InsertOneAsync(collection);
        return new LoginResult { Success = true, SessionKey = sessionKey };
    }

    /// <summary>
    /// Logs in user if password and username are valid.
    /// Uses PasswordHasher class for password decryption
    /// </summary>
    /// <returns>LoginRefult containing sessionKey if succsesful</returns>
    public async Task<LoginResult> Login(string username, string password){
        User user = await GetUserByUsername(username);

        if (user is not null && PasswordHasher.VerifyPassword(password, user.Password))
        {
    

            //Get habit collection for updating missing dates
            var filter = Builders<HabitCollection>.Filter.Eq(hc => hc.Id, user.Id);
            HabitCollection collection = await _habitCollections
                .Find(filter)
                .Project<HabitCollection>(Builders<HabitCollection>.Projection.Include(hc => hc.Habits))
                .FirstOrDefaultAsync();
            List<UpdateDefinition<HabitCollection>> habitHistoryUpdates = [];
            UpdateDefinitionBuilder<HabitCollection> updateHabitCollection = Builders<HabitCollection>.Update;

            //Get the previous date time in the date class format for < and > comparisons
            DateTime today = DateTime.Today.Date;
            if (!DateTime.TryParse(user.LastLoginDate, out DateTime lastLogin))
                throw new Exception("Date was not parsed properly");
            lastLogin = lastLogin.Date;

            HistoricalDate datedHabits = new()
            { 
                AllHabitsCompleted=false
            };
            foreach (Habit habit in collection.Habits)
                datedHabits.Habits[habit.Id!] = habit;

            /*For every day there has not been a login and today, set the habit history as the blank slate
            of incomplete haibts*/
            while (lastLogin <= today)
            {
                string thisMonth = lastLogin.ToString("yyyy-MM");
                string thisDay = lastLogin.ToString("dd");
                //add the dict to the db
                habitHistoryUpdates.Add(
                    updateHabitCollection.Set($"HabitHistory.{thisMonth}.{thisDay}", datedHabits)
                );

                lastLogin = lastLogin.AddDays(1);
            }
            //complete all updated on the dictionary
            await _habitCollections.UpdateOneAsync(filter, updateHabitCollection.Combine(habitHistoryUpdates));

            string sessionKey = GenerateSessionKey();
            await _users.UpdateOneAsync(
                u => u.Username.Equals(username),
                update.Combine(
                    update.Set(u => u.SessionKey, sessionKey),
                    update.Set(u => u.LastLoginDate, today.ToString("yyyy-MM-dd"))
                )
            );

            return new LoginResult { Success = true, SessionKey = sessionKey };
        }
        return new LoginResult{Success = false};
    }

    public async Task<bool> Logout(string sessionKey){
        User user = await GetUserBySessionKey(sessionKey);
        if(user is not null && user.SessionKey.Equals(sessionKey)){
            await _users.UpdateOneAsync(u=>u.SessionKey.Equals(sessionKey),update.Set(u=>u.SessionKey, ""));
            return true;
        }
        return false;
    }

    /// <summary>
    /// Indexes session keys to make lookup faster, only used on server startup
    /// </summary>
    public void CreateSessionKeyIndexes()
    {
        var indexKeys = Builders<User>.IndexKeys.Ascending(u => u.SessionKey);
        var indexModel = new CreateIndexModel<User>(indexKeys);
        _users.Indexes.CreateOne(indexModel); // Run once on startup
    }
}