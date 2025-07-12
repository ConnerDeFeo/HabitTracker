using Server.dtos;

namespace Server.service.interfaces;

public interface IGoogleAuthService
{ 
    Task<LoginResult> CreateUser(string jwt, string deviceId);
    Task<LoginResult> Login(string jwt, string deviceId);
}