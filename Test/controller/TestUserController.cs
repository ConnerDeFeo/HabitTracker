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
        .Setup(us => us.CreateUser("ConnerDeFeo","12345678"))
        .Returns(Task.FromResult(new LoginResult{Success=true, SessionKey="TestSessionKey"}));

        MockUserService
        .Setup(us => us.CreateUser("ConnerDeFeo","1234567"))
        .Returns(Task.FromResult(new LoginResult{Success=false}));

        MockUserService
        .Setup(us => us.Login("ConnerDeFeo","12345678"))
        .Returns(Task.FromResult(new LoginResult{Success=true, SessionKey="TestSessionKey"}));

        MockUserService
        .Setup(us => us.Login("ConnerDeFeo","1234567"))
        .Returns(Task.FromResult(new LoginResult{Success=false}));

        MockUserService
        .Setup(us => us.GetUser("TestSessionKey"))
        .Returns(Task.FromResult(new User{Username="ConnerDeFeo",SessionKey="TestSessionKey",Password=""}));

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
    public async Task TestGetUserFailure(){
        IActionResult Result = await Controller.GetUser("TestSessionKeyInvalid");
        var ConflictResult = Assert.IsType<ConflictResult>(Result);
        Assert.Equal(409,ConflictResult.StatusCode);
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

}