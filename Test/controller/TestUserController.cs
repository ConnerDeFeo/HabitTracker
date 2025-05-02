namespace Test.controller;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using Server.service;
using Server.controller;
using Server.model;
using Moq;


public class TestUserController{

    UserController controller;

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

        controller = new UserController(MockUserService.Object);
    }

    [Fact]
    public async Task TestGetUser(){
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers["Cookie"] = "SessionKey=TestSessionKey";
        controller.ControllerContext.HttpContext = httpContext;

        IActionResult Result = await controller.GetUser();
        var OkResult = Assert.IsType<OkObjectResult>(Result);
        var UserResult = Assert.IsType<UserDto>(OkResult.Value);
        Assert.Equal(200,OkResult.StatusCode);
        Assert.Equal("ConnerDeFeo",UserResult.Username);
    }

    [Fact]
    public async Task TestGetUserFail(){
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers["Cookie"] = "SessionKey=TestSessionKeyInvalid";
        controller.ControllerContext.HttpContext = httpContext;

        IActionResult Result = await controller.GetUser();
        var UnauthorizedResult = Assert.IsType<UnauthorizedResult>(Result);
        Assert.Equal(401,UnauthorizedResult.StatusCode);
    }

    [Fact]
    public async Task TestCreateUser(){
        IActionResult Result = await controller.CreateUser(new User{Username="ConnerDeFeo", Password="12345678"});
        var OkResult = Assert.IsType<OkObjectResult>(Result);
        var LoginResult = Assert.IsType<LoginResult>(OkResult.Value);
        Assert.Equal(200,OkResult.StatusCode);
        Assert.True(LoginResult.Success);
        Assert.Equal("TestSessionKey",LoginResult.SessionKey);
    }

    [Fact]
    public async Task TestCreateUserFail(){
        IActionResult Result = await controller.CreateUser(new User{Username="ConnerDeFeo", Password="1234567"});
        var ConflictResult = Assert.IsType<ConflictObjectResult>(Result);
        var LoginResult = Assert.IsType<LoginResult>(ConflictResult.Value);
        Assert.Equal(409,ConflictResult.StatusCode);
        Assert.False(LoginResult.Success);
        Assert.Equal("",LoginResult.SessionKey);
    }

    [Fact]
    public async Task TestLogin(){
        IActionResult Result = await controller.Login(new User{Username="ConnerDeFeo", Password="12345678"});
        var OkResult = Assert.IsType<OkObjectResult>(Result);
        var LoginResult = Assert.IsType<LoginResult>(OkResult.Value);
        Assert.Equal(200,OkResult.StatusCode);
        Assert.True(LoginResult.Success);
        Assert.Equal("TestSessionKey",LoginResult.SessionKey);
    }

    [Fact]
    public async Task TestLoginFail(){
        IActionResult Result = await controller.Login(new User{Username="ConnerDeFeo", Password="1234567"});
        var UnauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(Result);
        var LoginResult = Assert.IsType<LoginResult>(UnauthorizedResult.Value);
        Assert.Equal(401,UnauthorizedResult.StatusCode);
        Assert.False(LoginResult.Success);
        Assert.Equal("",LoginResult.SessionKey);
    }

    [Fact]
    public async Task TestLogout(){
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers["Cookie"] = "SessionKey=TestSessionKey";
        controller.ControllerContext.HttpContext = httpContext;

        IActionResult result = await controller.Logout();

        var okResult = Assert.IsType<OkResult>(result);
        Assert.Equal(200, okResult.StatusCode);
    }

    [Fact]
    public async Task TestLogoutFail(){
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers["Cookie"] = "SessionKey=TestSessionKeyInvalid";
        controller.ControllerContext.HttpContext = httpContext;

        IActionResult result = await controller.Logout();

        var unauthorizedResult = Assert.IsType<UnauthorizedResult>(result);
        Assert.Equal(401, unauthorizedResult.StatusCode);
    }

}