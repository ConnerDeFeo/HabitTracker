namespace Server.service.concrete;

using System.Text.RegularExpressions;
using MongoDB.Bson;
using MongoDB.Driver;
using Server.dtos;
using Server.model.habit;
using Server.model.user;
using Server.service.interfaces;
using Server.service.utils;

public class MongoSocialDataService(IMongoDatabase database) : ISocialDataService
{
    private readonly IMongoCollection<User> _users = database.GetCollection<User>("Users");
    private readonly IMongoCollection<HabitCollection> _habitCollections = database.GetCollection<HabitCollection>("HabitCollection");

    public async Task<Dictionary<string, string>?> GetFriends(string sessionKey)
    {
        User? user = await UserUtils.GetUserBySessionKey(sessionKey, _users);
        if (user is not null)
            return user.Friends;
        return null;
    }

    //Returns the profile habits of a given friend if they are friends
    public async Task<Profile?> GetProfile(string sessionKey, string username)
    {
        User? user = await UserUtils.GetUserBySessionKey(sessionKey, _users);
        User? friend = await UserUtils.GetUserByUsername(username, _users);
        if (user is not null && friend is not null && user.Id != friend.Id)
        {
            if (!user.Friends.ContainsKey(friend.Username))
                return null;

            string friendId = friend.Id!;
            HabitCollection? collection = await _habitCollections
                .Find(hc => hc.Id!.Equals(friend.Id))
                .FirstOrDefaultAsync();

            if (collection is not null)
                return UserUtils.GetProfile(collection,friend);
        }
        return null;
    }

    /// <summary>
    /// Finds user with a given phrase in their username case in-sensitive
    /// </summary>
    /// <param name="sessionKey">sessionkey of user checking</param>
    /// <param name="phrase">phrase being searches</param>
    /// <returns>List of users (max 5) that contain the given phrase</returns>
    public async Task<Dictionary<string, string>?> FindUser(string sessionKey, string phrase)
    {
        User? user = await UserUtils.GetUserBySessionKey(sessionKey, _users);
        if (user is null)
            return null;

        //Find the given users based on the name search excluding the user themselves
        Dictionary<string, string> usersAndProfilePics = [];
        var regexFilter = BuilderUtils.userFilter.Regex(
            u => u.Username,
            new BsonRegularExpression($"{Regex.Escape(phrase)}", "i")
        );
        var excludeCurrentUser = BuilderUtils.userFilter.Ne(u => u.Id, user.Id);
        var finalFilter = BuilderUtils.userFilter.And(regexFilter, excludeCurrentUser);

        List<User> users = await _users.Find(finalFilter).ToListAsync();
        foreach (User u in users)
            usersAndProfilePics[u.Username] = u.Id!;

        return usersAndProfilePics;
    }

    public async Task<Dictionary<string, string>?> GetRandomUsers(string sessionKey)
    {
        User? user = await UserUtils.GetUserBySessionKey(sessionKey, _users);
        if (user is null)
            return null;
        
        //id cannot be equal to the users id, need to convert to a mongo object id for this exclusion to work
        var matchStage = new BsonDocument("$match", new BsonDocument("_id", new BsonDocument("$ne", new ObjectId(user.Id!.ToString()))));
        //Sample 5 randoms not including user
        var sampleStage = new BsonDocument("$sample", new BsonDocument("size", 5));

        BsonDocument[] pipeline = [matchStage, sampleStage];

        var excludeCurrentUser = BuilderUtils.userFilter.Ne(u => u.Id, user.Id);

        List<User> randomUsers = await _users.Aggregate<User>(pipeline).ToListAsync();
        Dictionary<string, string> usernameToProfilePic = [];
        foreach (User u in randomUsers)
            usernameToProfilePic[u.Username] = u.Id!;
        
        return usernameToProfilePic;
        
    }
}
