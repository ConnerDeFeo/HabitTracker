namespace Server.service.concrete;

using System.Text.RegularExpressions;
using MongoDB.Bson;
using MongoDB.Driver;
using Server.dtos;
using Server.model.habit;
using Server.model.user;
using Server.service.interfaces;
using Server.service.utils;

public class MongoFriendService(IMongoDatabase database) : IFriendService
{
    private readonly IMongoCollection<User> _users = database.GetCollection<User>("Users");
    private readonly IMongoCollection<HabitCollection> _habitCollections = database.GetCollection<HabitCollection>("HabitCollection");

    //Send friend request to given user
    public async Task<bool> SendFriendRequest(string sessionKey, string friendUsername)
    {
        User? user = await UserUtils.GetUserBySessionKey(sessionKey, _users);
        User? friend = await UserUtils.GetUserByUsername(friendUsername, _users);
        if (user is not null && friend is not null && user.Id != friend.Id)
        {
            //User cannot already be friends, have a pending request, or have sent a request
            if (user.Friends.ContainsKey(friend.Username) || user.FriendRequestsSent.Contains(friend.Username) || user.FriendRequests.ContainsKey(friend.Username))
                return false;

            //Add to the other persons friend requests
            await _users.UpdateOneAsync(
                u => u.Id == friend.Id,
                BuilderUtils.userUpdate.
                Set($"FriendRequests.{user.Username}", user.Id)
            );

            //Add to the users friend request sent
            await _users.UpdateOneAsync(
                u => u.Id == user.Id,
                BuilderUtils.userUpdate.
                Push($"FriendRequestsSent", friend.Username)
            );

            return true;
        }
        return false;
    }

    //Unsends friend request to given user
    public async Task<bool> UnSendFriendRequest(string sessionKey, string friendUsername)
    {
        User? user = await UserUtils.GetUserBySessionKey(sessionKey, _users);
        User? friend = await UserUtils.GetUserByUsername(friendUsername, _users);
        if (user is not null && friend is not null && user.Id != friend.Id)
        {
            string friendId = friend.Id!;

            if (!friend.FriendRequests.ContainsKey(user.Username))
                return false;

            //Unset friend request for the other person
            await _users.UpdateOneAsync(
                u => u.Id == friend.Id,
                BuilderUtils.userUpdate.
                Unset($"FriendRequests.{user.Username}")
            );

            //Remove from friend request sent
            await _users.UpdateOneAsync(
                u => u.Id == user.Id,
                BuilderUtils.userUpdate.
                PullFilter(u => u.FriendRequestsSent, username => username == friend.Username)
            );

            return true;
        }
        return false;
    }

    //Removes the given friend from friend requests and 
    public async Task<Dictionary<string, string>?> AcceptFriendRequest(string sessionKey, string friendUsername)
    {
        User? user = await UserUtils.GetUserBySessionKey(sessionKey, _users);
        User? friend = await UserUtils.GetUserByUsername(friendUsername, _users);
        if (user is not null && friend is not null && user.Id != friend.Id)
        {
            string friendId = friend.Id!;
            if (!user.FriendRequests.ContainsKey(friend.Username))
                return null;

            BuilderUtils.userOptions.ReturnDocument = ReturnDocument.After;

            //Add to friend list and remove fromt pending requests
            User? updatedUser = await _users.FindOneAndUpdateAsync(
                u => u.Id == user.Id,
                BuilderUtils.userUpdate.
                Set($"Friends.{friend.Username}", friend.Id).
                Unset($"FriendRequests.{friend.Username}"),
                BuilderUtils.userOptions
            );

            //Add to other persons friend list and pull request from their friend requests sent
            await _users.UpdateOneAsync(
                u => u.Id == friendId,
                BuilderUtils.userUpdate.
                Set($"Friends.{user.Username}", user.Id).
                PullFilter(u => u.FriendRequestsSent, username => username == user.Username)
            );

            return updatedUser.Friends;
        }
        return null;
    }

    public async Task<Dictionary<string, string>?> RemoveFriend(string sessionKey, string friendUsername)
    {
        User? user = await UserUtils.GetUserBySessionKey(sessionKey, _users);
        User? friend = await UserUtils.GetUserByUsername(friendUsername, _users);
        if (user is not null && friend is not null && user.Id != friend.Id)
        {
            string friendId = friend.Id!;
            if (!user.Friends.ContainsKey(friend.Username))
                return null;

            await _users.UpdateOneAsync(
                u => u.Id == friendId,
                BuilderUtils.userUpdate.
                Unset($"Friends.{user.Username}")
            );

            BuilderUtils.userOptions.ReturnDocument = ReturnDocument.After;
            User? updatedUser = await _users.FindOneAndUpdateAsync(
                u => u.Id == user.Id,
                BuilderUtils.userUpdate.
                Unset($"Friends.{friend.Username}"),
                BuilderUtils.userOptions
            );

            return updatedUser.Friends;
        }
        return null;
    }
    public async Task<bool> RejectFriendRequest(string sessionKey, string friendUsername)
    {
        User? user = await UserUtils.GetUserBySessionKey(sessionKey, _users);
        User? friend = await UserUtils.GetUserByUsername(friendUsername, _users);
        if (user is not null && friend is not null && user.Id != friend.Id)
        {
            if (!user.FriendRequests.ContainsKey(friend.Username))
                return false;

            string userId = user.Id!;
            string friendId = friend.Id!;

            await _users.UpdateOneAsync(
                u => u.Id == userId,
                BuilderUtils.userUpdate.
                Unset($"FriendRequests.{friend.Username}")
            );

            await _users.UpdateOneAsync(
                u => u.Id == friendId,
                BuilderUtils.userUpdate.
                PullFilter(u => u.FriendRequestsSent, username => username == user.Username)
            );

            return true;
        }
        return false;
    }
    public async Task<Dictionary<string, string>?> GetFriends(string sessionKey)
    {
        User? user = await UserUtils.GetUserBySessionKey(sessionKey, _users);
        if (user is not null)
            return user.Friends;
        return null;
    }

    //Returns the profile habits of a given friend if they are friends
    public async Task<ProfileHabits?> GetFriendProfile(string sessionKey, string friendUsername)
    {
        User? user = await UserUtils.GetUserBySessionKey(sessionKey, _users);
        User? friend = await UserUtils.GetUserByUsername(friendUsername, _users);
        if (user is not null && friend is not null && user.Id != friend.Id)
        {
            if (!user.Friends.ContainsKey(friend.Username))
                return null;

            string friendId = friend.Id!;
            HabitCollection? collection = await _habitCollections
                .Find(hc => hc.Id!.Equals(friend.Id))
                .FirstOrDefaultAsync();

            if (collection is not null)
                return UserUtils.GetProfileHabits(collection);
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
