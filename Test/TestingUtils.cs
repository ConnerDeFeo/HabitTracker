namespace Test;
using Server.service.interfaces;
using Server.dtos;

public class TestingUtils(IUserService userService)
{
    private IUserService _userService = userService;
    public async Task<LoginResult> CreateUser(string username)
    {
        return await _userService.CreateUser(new LoginRequest { Username=username, Password="12345678",DeviceId="1234",Email="something" });
    }
}