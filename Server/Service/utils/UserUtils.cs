using MongoDB.Driver;
using Server.model.user;

namespace Server.service.utils;

public static class UserUtils
{ 

    public static async Task<User?> GetUserByUsername(string username, IMongoCollection<User> _users)
    {
        var Filter = BuilderUtils.userFilter.Eq(u => u.Username, username);

        return await _users.Find(Filter).FirstOrDefaultAsync();
    }

    public static async Task<User?> GetUserBySessionKey(string sessionKey, IMongoCollection<User> _users){
        var Filter = BuilderUtils.userFilter.Eq(u => u.SessionKey, sessionKey);

        return await _users.Find(Filter).FirstOrDefaultAsync();
    }
}