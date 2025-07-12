using Server.dtos;

namespace Server.service.interfaces;

public interface IGoogleAuthService
{ 
    Task<LoginResult> CreateUser(string jwtToken, string deviceId);
    Task<LoginResult> Login(string jwtToken, string deviceId);
}