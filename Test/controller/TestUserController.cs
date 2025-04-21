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
        .Returns(Task.FromResult(new LoginResult{Success=true, Token="TestToken"}));

        MockUserService
        .Setup(us => us.CreateUser("ConnerDeFeo","1234567"))
        .Returns(Task.FromResult(new LoginResult{Success=false}));

        MockUserService
        .Setup(us => us.Login("ConnerDeFeo","12345678"))
        .Returns(Task.FromResult(new LoginResult{Success=true, Token="TestToken"}));

        MockUserService
        .Setup(us => us.Login("ConnerDeFeo","1234567"))
        .Returns(Task.FromResult(new LoginResult{Success=false}));

        Controller = new UserController(MockUserService.Object);
    }

    [Fact]
    public async Task TestCreateUser(){
        IActionResult result = await Controller.CreateUser(new User{Username="ConnerDeFeo", Password="12345678"});
        var OkResult = Assert.IsType<OkObjectResult>(result);
        var LoginResult = Assert.IsType<LoginResult>(OkResult.Value);
        Assert.Equal(200,OkResult.StatusCode);
        Assert.True(LoginResult.Success);
        Assert.Equal("TestToken",LoginResult.Token);
    }

    [Fact]
    public async Task TestCreateUserFail(){
        IActionResult result = await Controller.CreateUser(new User{Username="ConnerDeFeo", Password="1234567"});
        var ConflictResult = Assert.IsType<ConflictObjectResult>(result);
        var LoginResult = Assert.IsType<LoginResult>(ConflictResult.Value);
        Assert.Equal(409,ConflictResult.StatusCode);
        Assert.False(LoginResult.Success);
        Assert.Null(LoginResult.Token);
    }

    [Fact]
    public async Task TestLogin(){
        IActionResult result = await Controller.Login(new User{Username="ConnerDeFeo", Password="12345678"});
        var OkResult = Assert.IsType<OkObjectResult>(result);
        var LoginResult = Assert.IsType<LoginResult>(OkResult.Value);
        Assert.Equal(200,OkResult.StatusCode);
        Assert.True(LoginResult.Success);
        Assert.Equal("TestToken",LoginResult.Token);
    }

    [Fact]
    public async Task TestLoginFail(){
        IActionResult result = await Controller.Login(new User{Username="ConnerDeFeo", Password="1234567"});
        var UnauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
        var LoginResult = Assert.IsType<LoginResult>(UnauthorizedResult.Value);
        Assert.Equal(401,UnauthorizedResult.StatusCode);
        Assert.False(LoginResult.Success);
        Assert.Null(LoginResult.Token);
    }

}