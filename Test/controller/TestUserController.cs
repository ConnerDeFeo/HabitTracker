namespace Test.controller;
using Microsoft.AspNetCore.Mvc;
using Server.service;
using Server.controller;
using Server.model;
using Moq;


public class TestUserController{

    UserController controller;

    public TestUserController(){
        var mockUserService = new Mock<IUserService>();

        mockUserService
        .Setup(us => us.CreateUser("ConnerDeFeo","12345678"))
        .Returns(Task.FromResult(new LoginResult{Success=true, Token="TestToken"}));

        mockUserService
        .Setup(us => us.CreateUser("ConnerDeFeo","1234567"))
        .Returns(Task.FromResult(new LoginResult{Success=false}));

        controller = new UserController(mockUserService.Object);
    }

    [Fact]
    public async Task TestCreateUser(){
        IActionResult result = await controller.CreateUser(new User{Username="ConnerDeFeo", Password="12345678"});
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200,okResult.StatusCode);
    }

}