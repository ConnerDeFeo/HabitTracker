namespace Server.service;
using Server.model;

public interface IUserService{
    Task<User?> GetUser(string sessionKey);
    Task<LoginResult> CreateUser(string username, string password);
    Task<LoginResult> Login(string username, string password);

    Task<bool> Logout(string sessionKey);
}