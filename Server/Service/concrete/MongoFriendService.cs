namespace Server.service.concrete;

using MongoDB.Driver;
using Server.dtos;
using Server.model.user;
using Server.service.interfaces;
using Server.service.utils;

public class MongoFriendService(IMongoDatabase database) : IFriendService
{
    private readonly IMongoCollection<User> _users = database.GetCollection<User>("Users");

    //Send friend request to given user
    public async Task<bool> SendFriendRequest(string sessionKey, string username)
    {
        User? user = await UserUtils.GetUserBySessionKey(sessionKey, _users);
        User? friend = await UserUtils.GetUserByUsername(username, _users);
        if (user is not null && friend is not null && user.Id != friend.Id)
        {

            if (user.Friends.ContainsKey(friend.Username) || user.FriendRequestsSent.Contains(friend.Username) || user.FriendRequests.ContainsKey(friend.Username))
                return false;

            await _users.UpdateOneAsync(
                u => u.Id == friend.Id,
                BuilderUtils.userUpdate.
                Set($"FriendRequests.{user.Username}", user.ProfilePhotoKey)
            );

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
    public async Task<bool> UnSendFriendRequest(string sessionKey, string username)
    {
        User? user = await UserUtils.GetUserBySessionKey(sessionKey, _users);
        User? friend = await UserUtils.GetUserByUsername(username, _users);
        if (user is not null && friend is not null && user.Id != friend.Id)
        {
            string friendId = friend.Id!;

            if (!friend.FriendRequests.ContainsKey(user.Username))
                return false;

            await _users.UpdateOneAsync(
                    u => u.Id == friend.Id,
                    BuilderUtils.userUpdate.
                    Unset($"FriendRequests.{user.Username}")
            );

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
    public async Task<Dictionary<string,string?>?> AcceptFriendRequest(string sessionKey, string username)
    {
        User? user = await UserUtils.GetUserBySessionKey(sessionKey, _users);
        User? friend = await UserUtils.GetUserByUsername(username, _users);
        if (user is not null && friend is not null && user.Id != friend.Id)
        {
            string friendId = friend.Id!;

            if (!user.FriendRequests.ContainsKey(user.Username))
                return null;

            BuilderUtils.userOptions.ReturnDocument = ReturnDocument.After;

            User? updatedUser = await _users.FindOneAndUpdateAsync(
                u => u.Id == user.Id,
                BuilderUtils.userUpdate.
                Set($"Friends.{friend.Username}", friend.ProfilePhotoKey).
                Unset($"FriendRequests.{friend.Username}"),
                BuilderUtils.userOptions
            );

            await _users.UpdateOneAsync(
                u => u.Id == friendId,
                BuilderUtils.userUpdate.
                Set($"Friends.{user.Username}", user.ProfilePhotoKey)
            );

            return updatedUser.Friends;
        }
        return null;
    }

    public async Task<Dictionary<string,string?>?> RemoveFriend(string sessionKey, string username)
    {
        return [];
    }
    public async Task<bool> RejectFriend(string sessionKey, string username)
    {
        return false;
    }
    public async Task<Dictionary<string,string?>?> GetFriends(string sessionKey)
    {
        return [];
    }
    public async Task<ProfileHabits> GetFriendProfile(string sessionKey)
    {
        return new ProfileHabits();
    }

}
