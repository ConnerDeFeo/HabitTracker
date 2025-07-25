namespace Server.service.interfaces;
using Server.dtos;


/// <summary>
/// Interface for the interactions with User service classes.
/// </summary>
public interface IUserService
{
    Task<UserDto?> GetUser(string sessionKey);
    Task<LoginResult> CreateUser(dtos.LoginRequest request);
    Task<bool> ChangeUsername(string sessionKey, string newUsername);
    Task<LoginResult> Login(dtos.LoginRequest request);
    Task<bool> Logout(string sessionKey);
    Task<Profile?> GetProfile(string sessionKey);

}