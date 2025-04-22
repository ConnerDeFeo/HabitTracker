namespace Server.service;
using Server.model;

public interface IUserService{
    Task<User> GetUser(string SessionKey);
    Task<LoginResult> CreateUser(string Username, string Password);

    Task<LoginResult> Login(string Username, string Password);
}