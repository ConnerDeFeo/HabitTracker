namespace Server.service;
using MongoDB.Driver;
using Server.model;

public interface IUserService{
    Task<User> GetUser(string username);
    Task<LoginResult> CreateUser(string username, string password);

    Task<LoginResult> Login(string username, string password);
}