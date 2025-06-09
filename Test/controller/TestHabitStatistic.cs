namespace Test.controller;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using Microsoft.AspNetCore.Http;
using Server.service;
using Server.controller;
using Server.model.user;
using Server.model.habit;
using Moq;


public class TestHabitStatistic
{
    HabitStatisticController habitStatisticController;

    public TestHabitStatistic()
    {
        var mockHabitStatisticService = new Mock<IHabitStatisticService>();
        mockHabitStatisticService
        .Setup(hs => hs.GetHistoricalData(It.IsAny<string>(), It.IsAny<string>()))
        .Returns<string, string>((sessionKey, habitId) =>
            {
                if (sessionKey.Equals("TestSessionKey"))
                {
                    if (habitId.Equals("1234"))
                        return Task.FromResult<HistoricalData?>(new() { Habit = new Habit{Id = "1233"} });
                    return Task.FromResult<HistoricalData?>(null);
                }
                else
                    return Task.FromResult<HistoricalData?>(null);
            }
        );

        habitStatisticController = new HabitStatisticController(mockHabitStatisticService.Object);
    }

    private void SetValidSessionKey()
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers["Cookie"] = "SessionKey=TestSessionKey";
        habitStatisticController.ControllerContext.HttpContext = httpContext;
    }

    private void SetInvalidSessionKey()
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers["Cookie"] = "SessionKey=TestSessionKeyInvalid";
        habitStatisticController.ControllerContext.HttpContext = httpContext;
    }

    [Fact]
    public async Task TestGetHistoricalData()
    {
        SetValidSessionKey();
        IActionResult result = await habitStatisticController.GetHistoricalData("1234");
        var okResult = Assert.IsType<OkObjectResult>(result);
        var dataResult = Assert.IsType<HistoricalData>(okResult.Value);
        Assert.NotNull(dataResult.Habit);
    }

    [Fact]
    public async Task TestGetHistoricalDataInvalid()
    {
        SetInvalidSessionKey();
        IActionResult result = await habitStatisticController.GetHistoricalData("1234");
        Assert.IsType<NotFoundResult>(result);
        
        SetValidSessionKey();
        result = await habitStatisticController.GetHistoricalData("1233");
        Assert.IsType<NotFoundResult>(result);
    }

}