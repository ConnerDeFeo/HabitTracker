namespace Server.service;
using Server.model;

public interface IUserService{
    Task<User> GetUserPublic(string username);
    Task<LoginResult> CreateUser(string username, string password);

    Task<LoginResult> Login(string username, string password);
}