namespace Test.controller;
using Server.service;
using Server.model;
using Moq;


public class TestUserController{

    TestUserController controller;

    public TestUserController(){
        var mockUserService = new Mock<UserService>();
        mockUserService.Setup(us => us.CreateUser("ConnerDeFeo","12345678")).Returns(new LoginResult{Success=true, Token="TestToken"});
        mockUserService.Setup(us => us.CreateUser("ConnerDeFeo","1234567")).Returns(new LoginResult{Success=false});
        controller = new UserController(mockUserService.Object);
    }

    [Fact]
    public async Task TestCreateUser(){
        IActionResult result = await controller.CreateUser(new User{Username="ConnerDeFeo", Password="12345678"});
        Assert.Equals(200,result.status);
    }

}