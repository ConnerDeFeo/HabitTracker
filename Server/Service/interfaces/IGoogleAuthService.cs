using Server.dtos;

namespace Server.service.interfaces;

public interface IGoogleAuthService
{ 
    Task<LoginResult> Login(string jwt, string deviceId);
}