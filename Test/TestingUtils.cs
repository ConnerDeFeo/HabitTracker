namespace Test;
using Server.service.interfaces;
using Server.dtos;

public class TestingUtils(IUserService userService)
{
    private IUserService _userService = userService;
    private int Counter = 0;
    public async Task<LoginResult> CreateUser(string username)
    {
        return await
        _userService.CreateUser(new LoginRequest { Username = username, Password = "12345678", DeviceId = "1234", Email = $"something{Counter++}" });
    }
}