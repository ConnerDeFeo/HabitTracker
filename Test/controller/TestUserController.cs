namespace Test.controller;
using Microsoft.AspNetCore.Mvc;
using Server.service;
using Server.controller;
using Server.model;
using Moq;


public class TestUserController{

    UserController Controller;

    public TestUserController(){
        var MockUserService = new Mock<IUserService>();

        MockUserService
        .Setup(us => us.CreateUser(It.IsAny<string>(), It.IsAny<string>()))
        .Returns<string,string>((Username,Password)=>{
            if (Username.Equals("ConnerDeFeo") && Password.Equals("12345678"))
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
            .Returns<string, string>((Username, Password) =>
            {
                if (Username.Equals("ConnerDeFeo") && Password.Equals("12345678"))
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
            .Returns<string>((SessionKey) =>
            {
                if (SessionKey.Equals("TestSessionKey"))
                {
                    return Task.FromResult<User?>(new User { Username="ConnerDeFeo", SessionKey = "TestSessionKey" });
                }
                else
                {
                    return Task.FromResult<User?>(null);
                }
            });

        MockUserService
            .Setup(us => us.Logout(It.IsAny<string>(), It.IsAny<string>()))
            .Returns<string, string>((Username, SessionKey) =>
            {
                if (Username.Equals("ConnerDeFeo") && SessionKey.Equals("TestSessionKey"))
                {
                    return Task.FromResult(true);
                }
                else
                {
                    return Task.FromResult(false);
                }
            });

        Controller = new UserController(MockUserService.Object);
    }

    [Fact]
    public async Task TestGetUser(){
        IActionResult Result = await Controller.GetUser("TestSessionKey");
        var OkResult = Assert.IsType<OkObjectResult>(Result);
        var UserResult = Assert.IsType<User>(OkResult.Value);
        Assert.Equal(200,OkResult.StatusCode);
        Assert.Equal("ConnerDeFeo",UserResult.Username);
        Assert.Equal("TestSessionKey", UserResult.SessionKey);
        Assert.Equal("",UserResult.Password);
    }

    [Fact]
    public async Task TestGetUserFail(){
        IActionResult Result = await Controller.GetUser("TestSessionKeyInvalid");
        var UnauthorizedResult = Assert.IsType<UnauthorizedResult>(Result);
        Assert.Equal(401,UnauthorizedResult.StatusCode);
    }

    [Fact]
    public async Task TestCreateUser(){
        IActionResult Result = await Controller.CreateUser(new User{Username="ConnerDeFeo", Password="12345678"});
        var OkResult = Assert.IsType<OkObjectResult>(Result);
        var LoginResult = Assert.IsType<LoginResult>(OkResult.Value);
        Assert.Equal(200,OkResult.StatusCode);
        Assert.True(LoginResult.Success);
        Assert.Equal("TestSessionKey",LoginResult.SessionKey);
    }

    [Fact]
    public async Task TestCreateUserFail(){
        IActionResult Result = await Controller.CreateUser(new User{Username="ConnerDeFeo", Password="1234567"});
        var ConflictResult = Assert.IsType<ConflictObjectResult>(Result);
        var LoginResult = Assert.IsType<LoginResult>(ConflictResult.Value);
        Assert.Equal(409,ConflictResult.StatusCode);
        Assert.False(LoginResult.Success);
        Assert.Equal("",LoginResult.SessionKey);
    }

    [Fact]
    public async Task TestLogin(){
        IActionResult Result = await Controller.Login(new User{Username="ConnerDeFeo", Password="12345678"});
        var OkResult = Assert.IsType<OkObjectResult>(Result);
        var LoginResult = Assert.IsType<LoginResult>(OkResult.Value);
        Assert.Equal(200,OkResult.StatusCode);
        Assert.True(LoginResult.Success);
        Assert.Equal("TestSessionKey",LoginResult.SessionKey);
    }

    [Fact]
    public async Task TestLoginFail(){
        IActionResult Result = await Controller.Login(new User{Username="ConnerDeFeo", Password="1234567"});
        var UnauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(Result);
        var LoginResult = Assert.IsType<LoginResult>(UnauthorizedResult.Value);
        Assert.Equal(401,UnauthorizedResult.StatusCode);
        Assert.False(LoginResult.Success);
        Assert.Equal("",LoginResult.SessionKey);
    }

    [Fact]
    public async Task TestLogout(){
        IActionResult Result = await Controller.Logout(new User{Username="ConnerDeFeo", SessionKey="TestSessionKey"});
        var OkResult = Assert.IsType<OkResult>(Result);
        Assert.Equal(200,OkResult.StatusCode);
    }

    [Fact]
    public async Task TestLogoutFail(){
        IActionResult Result = await Controller.Logout(new User{Username="ConnerDeFeo", SessionKey="TestSessionKeyInvalid"});
        var UnauthorizedResult = Assert.IsType<UnauthorizedResult>(Result);
        Assert.Equal(401,UnauthorizedResult.StatusCode);
    }

}