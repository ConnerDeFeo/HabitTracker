namespace Test.controller;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using Microsoft.AspNetCore.Http;
using Server.service.interfaces;
using Server.controller;
using Server.model.user;
using Server.model.habit;
using Server.dtos;
using Moq;


public class TestUser
{

    UserController userController;
    string dateCreated;

    public TestUser()
    {
        var MockUserService = new Mock<IUserService>();
        dateCreated = DateTime.UtcNow.ToString("yyyy-MM-dd");

        MockUserService
        .Setup(us => us.CreateUser(It.IsAny<LoginRequest>()))
        .Returns<LoginRequest>((request) =>
        {
            if (request.Username.Equals("ConnerDeFeo") && request.Password.Equals("12345678"))
            {
                return Task.FromResult(new LoginResult
                {
                    SessionKey = "TestSessionKey",
                    User = new UserDto { Username = request.Username, DateCreated = dateCreated }
                });
            }
            else
            {
                return Task.FromResult(new LoginResult
                {
                    SessionKey = "",
                });
            }
        }
        );

        MockUserService
            .Setup(us => us.Login(It.IsAny<LoginRequest>()))
            .Returns<LoginRequest>((request) =>
            {
                if (request.Username.Equals("ConnerDeFeo") && request.Password.Equals("12345678"))
                {
                    return Task.FromResult(new LoginResult
                    {
                        SessionKey = "TestSessionKey",
                        User = new UserDto { Username = request.Username, DateCreated = dateCreated }
                    });
                }
                else
                {
                    return Task.FromResult(new LoginResult { SessionKey = "" });
                }
            });

        MockUserService
            .Setup(us => us.GetUser(It.IsAny<string>()))
            .Returns<string>((sessionKey) =>
            {
                if (sessionKey.Equals("TestSessionKey"))
                {
                    return Task.FromResult<UserDto?>(new UserDto { Username = "ConnerDeFeo", DateCreated = dateCreated });
                }
                else
                {
                    return Task.FromResult<UserDto?>(null);
                }
            });

        MockUserService
            .Setup(us => us.GetProfile(It.IsAny<string>()))
            .Returns<string>((sessionKey) =>
            {
                if (sessionKey.Equals("TestSessionKey"))
                {
                    return Task.FromResult<Profile?>(new Profile());
                }
                else
                {
                    return Task.FromResult<Profile?>(null);
                }
            });

        MockUserService
            .Setup(us => us.Logout(It.IsAny<string>()))
            .Returns<string>((sessionKey) =>
            {
                if (sessionKey.Equals("TestSessionKey"))
                {
                    return Task.FromResult(true);
                }
                else
                {
                    return Task.FromResult(false);
                }
            });

        MockUserService
            .Setup(us => us.ChangeUsername(It.IsAny<string>(), It.IsAny<string>()))
            .Returns<string, string>((sessionKey, username) =>
            {
                if (sessionKey.Equals("TestSessionKey") && username.Equals("new"))
                {
                    return Task.FromResult(true);
                }
                else
                {
                    return Task.FromResult(false);
                }
            });

        userController = new UserController(MockUserService.Object);
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers["Cookie"] = "SessionKey=TestSessionKeyInvalid";
        userController.ControllerContext.HttpContext = httpContext;
    }

    private void SetValidSessionKey()
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers["Cookie"] = "SessionKey=TestSessionKey";
        userController.ControllerContext.HttpContext = httpContext;
    }

    private void SetInvalidSessionKey()
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers["Cookie"] = "SessionKey=TestSessionKeyInvalid";
        userController.ControllerContext.HttpContext = httpContext;
    }

    [Fact]
    public async Task TestGetUser()
    {
        SetValidSessionKey();
        IActionResult result = await userController.GetUser();
        var OkResult = Assert.IsType<OkObjectResult>(result);
        var UserResult = Assert.IsType<UserDto>(OkResult.Value);
        Assert.Equal(200, OkResult.StatusCode);
        Assert.Equal("ConnerDeFeo", UserResult.Username);
    }

    [Fact]
    public async Task TestGetUserFail()
    {
        SetInvalidSessionKey();

        IActionResult result = await userController.GetUser();
        Assert.IsType<UnauthorizedResult>(result);
    }

    [Fact]
    public async Task TestCreateUser()
    {
        IActionResult result = await userController.CreateUser(new LoginRequest { Username = "ConnerDeFeo", Password = "12345678", DeviceId = "1234" });
        var OkResult = Assert.IsType<OkObjectResult>(result);
        var LoginResult = Assert.IsType<UserDto>(OkResult.Value);
        Assert.Equal(200, OkResult.StatusCode);
    }

    [Fact]
    public async Task TestCreateUserFail()
    {
        IActionResult result = await userController.CreateUser(new LoginRequest { Username = "ConnerDeFeo", Password = "1234567", DeviceId = "1234" });
        var ConflictResult = Assert.IsType<ConflictObjectResult>(result);
        var LoginResult = Assert.IsType<LoginResult>(ConflictResult.Value);
        Assert.Equal("", LoginResult.SessionKey);
    }

    [Fact]
    public async Task TestLogin()
    {
        IActionResult result = await userController.Login(new LoginRequest { Username = "ConnerDeFeo", Password = "12345678", DeviceId = "1234" });
        var OkResult = Assert.IsType<OkObjectResult>(result);
        var LoginResult = Assert.IsType<UserDto>(OkResult.Value);
        Assert.Equal(200, OkResult.StatusCode);
    }

    [Fact]
    public async Task TestLoginFail()
    {
        IActionResult result = await userController.Login(new LoginRequest { Username = "ConnerDeFeo", Password = "1234567", DeviceId = "1234" });
        var UnauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
        var LoginResult = Assert.IsType<LoginResult>(UnauthorizedResult.Value);
        Assert.Equal("", LoginResult.SessionKey);
    }

    [Fact]
    public async Task TestLogout()
    {
        SetValidSessionKey();

        IActionResult result = await userController.Logout();

        var okResult = Assert.IsType<OkResult>(result);
        Assert.Equal(200, okResult.StatusCode);
    }

    [Fact]
    public async Task TestLogoutFail()
    {
        SetInvalidSessionKey();

        IActionResult result = await userController.Logout();

        Assert.IsType<UnauthorizedResult>(result);
    }

    [Fact]
    public async Task TestGetUserProfile()
    {
        SetValidSessionKey();

        IActionResult result = await userController.GetUserProfile();
        var okResult = Assert.IsType<OkObjectResult>(result);
        var profile = Assert.IsType<Profile>(okResult.Value);
        Assert.Equal(200, okResult.StatusCode);
    }

    [Fact]
    public async Task TestGetUserProfileFail()
    {
        SetInvalidSessionKey();

        IActionResult result = await userController.GetUserProfile();
        Assert.IsType<UnauthorizedResult>(result);
    }
    
    [Fact]
    public async Task TestChangeUsername()
    {
        SetValidSessionKey();

        IActionResult result = await userController.ChangeUsername("new");
        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task TestChangeUsernameFail()
    {
        SetInvalidSessionKey();

        IActionResult result = await userController.ChangeUsername("new");
        Assert.IsType<ConflictResult>(result);

        SetValidSessionKey();
        result = await userController.ChangeUsername("not new");
        Assert.IsType<ConflictResult>(result);
    }

}