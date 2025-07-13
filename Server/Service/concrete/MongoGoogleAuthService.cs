namespace Server.service.concrete;

using System.Security.Cryptography;
using Google.Apis.Auth;
using MongoDB.Bson;
using MongoDB.Driver;
using Server.dtos;
using Server.model.habit;
using Server.model.user;
using Server.service.interfaces;
using Server.service.utils;

public class MongoGoogleAuthService(IMongoDatabase database) : IGoogleAuthService
{
    private readonly IMongoCollection<User> _users = database.GetCollection<User>("Users");
    private readonly IMongoCollection<HabitCollection> _collection = database.GetCollection<HabitCollection>("HabitCollection");
    private readonly string thisMonth = DateTime.UtcNow.ToString("yyyy-MM");
    private readonly string thisDay = DateTime.UtcNow.ToString("dd");

    /// <summary>
    /// Checks that jwk token sent to backend is actually from google and returns the payload if so
    /// </summary>
    /// <param name="jwtToken">token sent from google login </param>
    /// <returns>payload containing all sensitive info about user</returns>
    private async static Task<GoogleJsonWebSignature.Payload?> VerifyGoogleTokenAsync(string jwt)
    {
        try
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings()
            {
                Audience = [Environment.GetEnvironmentVariable("GOOGLE_CLIENT_ID")]
            };
            var payload = await GoogleJsonWebSignature.ValidateAsync(jwt, settings);

            return payload;
        }
        catch (InvalidJwtException ex)
        {
            Console.WriteLine("Invalid token: " + ex.Message);
            return null;
        }
    }

    private static string GenerateSessionKey()
    {
        byte[] key = RandomNumberGenerator.GetBytes(32);
        return Convert.ToBase64String(key);
    }

    /// <summary>
    /// Logs in user based on google id token
    /// </summary>
    /// <param name="jwtToken">token sent from backend</param>
    /// <param name="deviceId">device sent from</param>
    /// <returns>login result if successful or not</returns>
    public async Task<LoginResult> Login(string jwt, string deviceId)
    {
        GoogleJsonWebSignature.Payload? payload = await VerifyGoogleTokenAsync(jwt);
        if (payload is null)
            return new LoginResult { SessionKey = "" };

        string sub = payload.Subject;
        User? user = await _users.Find(u => u.GoogleId == sub).FirstOrDefaultAsync();
        if (user is null)
            return await CreateUser(jwt, deviceId);

        await UserUtils.UpdateUserHistory(user, _collection);
        string newSessionKey = await UserUtils.UpdateSessionKeys(user, deviceId, _users);
        
         return new LoginResult
            {
                SessionKey = newSessionKey,
                User =
                new UserDto
                {
                    Username = user.Username,
                    DateCreated = user.DateCreated,
                    Id = user.Id!,
                    FriendRequests = user.FriendRequests,
                    Friends = user.Friends,
                    FriendRequestsSent = user.FriendRequestsSent
                }
            };
    }

    /// <summary>
    /// Creates user and gives them a google id token
    /// </summary>
    /// <param name="jwtToken">token sent from backend</param>
    /// <param name="deviceId">device sent from</param>
    /// <returns>login result if successful or not</returns>
    private async Task<LoginResult> CreateUser(string jwt, string deviceId)
    {
        GoogleJsonWebSignature.Payload? payload = await VerifyGoogleTokenAsync(jwt);
        if (payload is null)
            return new LoginResult { SessionKey = "" };

        string allowedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        int len = 20 - payload.Name.Length;
        if (len > 2)
            len -= 2; //just for some breathing room if able
        string username = $"{payload.Name}{RandomUtils.GenerateRandomString(len, allowedChars)}";
        string password = RandomUtils.GenerateRandomString(12, $"{allowedChars}0123456789!@#$%^&*()_-+=<>?");
        string email = payload.Email;

        string sessionKey = GenerateSessionKey();
        string id = ObjectId.GenerateNewId().ToString();
        Dictionary<string, string> sessionKeys = [];
        sessionKeys[sessionKey] = deviceId;
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
            DateCreated = today,
            GoogleId = payload.Subject
        };

        await _users.InsertOneAsync(user);
        HabitCollection collection = new() { Id = id };
        collection.HabitHistory[thisMonth] = [];
        collection.HabitHistory[thisMonth][thisDay] = new();
        await _collection.InsertOneAsync(collection);
        return new LoginResult { SessionKey = sessionKey, User = new UserDto { Username = username, DateCreated = today, Id = id } };
    }
}