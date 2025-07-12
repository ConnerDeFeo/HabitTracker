namespace Server.service.concrete;
using MongoDB.Driver;
using Server.model.habit;
using Server.model.user;
using Server.service.interfaces;
using Server.service.utils;

public class MongoFriendModificationService(IMongoDatabase database) : IFriendModificationService
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
}
