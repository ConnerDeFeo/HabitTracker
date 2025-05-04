namespace Test.controller;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using Microsoft.AspNetCore.Http;
using Server.service;
using Server.controller;
using Server.model;
using Moq;

public class TestHabitController
{
    HabitController habitController;

    public TestHabitController(){
        var MockHabitService = new Mock<IHabitService>();



        MockHabitService
        .Setup(hs => hs.CreateHabit(It.IsAny<string>(), It.IsAny<string>()))
        .Returns<string,string>((sessionKey,habit)=>{
            if (sessionKey.Equals("TestSessionKey"))
                {
                    return Task.FromResult<List<Habit>?>(new List<Habit>{new Habit{Name="TestHabit"}});
                }
                else
                {
                    return Task.FromResult<List<Habit>?>(null);
                }
            }
        );

        MockHabitService
        .Setup(hs => hs.GetHabits(It.IsAny<string>()))
        .Returns<string>((sessionKey)=>{
            if (sessionKey.Equals("TestSessionKey"))
                {
                    return Task.FromResult<List<Habit>?>(new List<Habit>{new Habit{Name="TestHabit"}});
                }
                else
                {
                    return Task.FromResult<List<Habit>?>(null);
                }
            }
        );
        habitController = new HabitController(MockHabitService.Object);
    }

    private void setValidSessionKey(){
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers["Cookie"] = "SessionKey=TestSessionKey";
        habitController.ControllerContext.HttpContext = httpContext;
    }

    private void setInvalidSessionKey(){
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers["Cookie"] = "SessionKey=TestSessionKeyInvalid";
        habitController.ControllerContext.HttpContext = httpContext;
    }

    [Fact]
    public async Task GetHabits(){
        setValidSessionKey();

        IActionResult result = await habitController.GetHabits();
        var okResult = Assert.IsType<OkObjectResult>(result);
        var habitResult = Assert.IsType<List<Habit>>(okResult.Value);
        Assert.Equal(200,okResult.StatusCode);
        Assert.NotEmpty(habitResult);
        Assert.Equal("TestHabit",habitResult[0].Name);
    }

    [Fact]
    public async Task GetHabitsInvalid(){
        setInvalidSessionKey();

        IActionResult result = await habitController.GetHabits();
        Assert.IsType<UnauthorizedResult>(result);
    }
}