namespace Test.controller;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using Microsoft.AspNetCore.Http;
using Server.service;
using Server.controller;
using Server.model.user;
using Server.model.habit;
using Moq;


public class TestUserController{

    UserController userController;

    public TestUserController(){
        var MockUserService = new Mock<IUserService>();

        MockUserService
        .Setup(us => us.CreateUser(It.IsAny<string>(), It.IsAny<string>()))
        .Returns<string,string>((username,password)=>{
            if (username.Equals("ConnerDeFeo") && password.Equals("12345678"))
                {
                    return Task.FromResult(new LoginResult { Success = true, SessionKey = "TestSessionKey" });
                }
                else
                {
                    return Task.FromResult(new LoginResult { Success = false, SessionKey = "" });
                }
            }
        );

        MockUserService
            .Setup(us => us.Login(It.IsAny<string>(), It.IsAny<string>()))
            .Returns<string, string>((username, password) =>
            {
                if (username.Equals("ConnerDeFeo") && password.Equals("12345678"))
                {
                    return Task.FromResult(new LoginResult { Success = true, SessionKey = "TestSessionKey" });
                }
                else
                {
                    return Task.FromResult(new LoginResult { Success = false, SessionKey = "" });
                }
            });

        MockUserService
            .Setup(us => us.GetUser(It.IsAny<string>()))
            .Returns<string>((sessionKey) =>
            {
                if (sessionKey.Equals("TestSessionKey"))
                {
                    return Task.FromResult<UserDto?>(new UserDto { Username="ConnerDeFeo" });
                }
                else
                {
                    return Task.FromResult<UserDto?>(null);
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

        userController = new UserController(MockUserService.Object);
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers["Cookie"] = "SessionKey=TestSessionKeyInvalid";
        userController.ControllerContext.HttpContext = httpContext;
    }

    private void setValidSessionKey(){
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers["Cookie"] = "SessionKey=TestSessionKey";
        userController.ControllerContext.HttpContext = httpContext;
    }

    private void setInvalidSessionKey(){
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers["Cookie"] = "SessionKey=TestSessionKeyInvalid";
        userController.ControllerContext.HttpContext = httpContext;
    }

    [Fact]
    public async Task TestGetUser(){
        setValidSessionKey();
        IActionResult result = await userController.GetUser();
        var OkResult = Assert.IsType<OkObjectResult>(result);
        var UserResult = Assert.IsType<UserDto>(OkResult.Value);
        Assert.Equal(200,OkResult.StatusCode);
        Assert.Equal("ConnerDeFeo",UserResult.Username);
    }

    [Fact]
    public async Task TestGetUserFail(){
        setInvalidSessionKey();

        IActionResult result = await userController.GetUser();
        Assert.IsType<UnauthorizedResult>(result);
    }

    [Fact]
    public async Task TestCreateUser(){
        IActionResult result = await userController.CreateUser(new User{Username="ConnerDeFeo", Password="12345678"});
        var OkResult = Assert.IsType<OkObjectResult>(result);
        var LoginResult = Assert.IsType<LoginResult>(OkResult.Value);
        Assert.Equal(200,OkResult.StatusCode);
        Assert.True(LoginResult.Success);
        Assert.Equal("TestSessionKey",LoginResult.SessionKey);
    }

    [Fact]
    public async Task TestCreateUserFail(){
        IActionResult result = await userController.CreateUser(new User{Username="ConnerDeFeo", Password="1234567"});
        var ConflictResult = Assert.IsType<ConflictObjectResult>(result);
        var LoginResult = Assert.IsType<LoginResult>(ConflictResult.Value);
        Assert.False(LoginResult.Success);
        Assert.Equal("",LoginResult.SessionKey);
    }

    [Fact]
    public async Task TestLogin(){
        IActionResult result = await userController.Login(new User{Username="ConnerDeFeo", Password="12345678"});
        var OkResult = Assert.IsType<OkObjectResult>(result);
        var LoginResult = Assert.IsType<LoginResult>(OkResult.Value);
        Assert.Equal(200,OkResult.StatusCode);
        Assert.True(LoginResult.Success);
        Assert.Equal("TestSessionKey",LoginResult.SessionKey);
    }

    [Fact]
    public async Task TestLoginFail(){
        IActionResult result = await userController.Login(new User{Username="ConnerDeFeo", Password="1234567"});
        var UnauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
        var LoginResult = Assert.IsType<LoginResult>(UnauthorizedResult.Value);
        Assert.False(LoginResult.Success);
        Assert.Equal("",LoginResult.SessionKey);
    }

    [Fact]
    public async Task TestLogout(){
        setValidSessionKey();

        IActionResult result = await userController.Logout();

        var okResult = Assert.IsType<OkResult>(result);
        Assert.Equal(200, okResult.StatusCode);
    }

    [Fact]
    public async Task TestLogoutFail(){
        setInvalidSessionKey();

        IActionResult result = await userController.Logout();

        Assert.IsType<UnauthorizedResult>(result);
    }

}