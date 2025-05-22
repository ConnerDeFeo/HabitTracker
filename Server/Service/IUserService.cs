namespace Server.service;
using Server.model.user;


/// <summary>
/// Interface for the interactions with User service classes.
/// </summary>
public interface IUserService
{
    Task<UserDto?> GetUser(string sessionKey);
    Task<LoginResult> CreateUser(string username, string password);
    Task<LoginResult> Login(string username, string password);
    Task<bool> Logout(string sessionKey);
    void CreateSessionKeyIndexes();

}