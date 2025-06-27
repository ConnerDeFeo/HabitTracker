namespace Test.controller;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using Microsoft.AspNetCore.Http;
using Server.service.interfaces;
using Server.controller;
using Server.model.user;
using Server.model.habit;
using Moq;


public class TestHabitHistory
{
    HabitHistoryController habitHistoryController;

    public TestHabitHistory()
    {
        var mockHabitHistoryService = new Mock<IHabitHistoryService>();
        mockHabitHistoryService
        .Setup(hs => hs.SetHabitCompletion(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()))
        .Returns<string, string, string, bool>((sessionKey, date, habitId, completed) =>
            {
                if (sessionKey.Equals("TestSessionKey"))
                {
                    if (habitId.Equals("1234") && date.Equals("2025-05-22"))
                        return Task.FromResult<bool>(true);
                    return Task.FromResult<bool>(false);
                }
                else
                    return Task.FromResult<bool>(false);
            }
        );

        mockHabitHistoryService
        .Setup(hs => hs.GetHabitHistoryByMonth(It.IsAny<string>(), It.IsAny<string>()))
        .Returns<string, string>((sessionKey, yyyyMM) =>
            {
                if (sessionKey.Equals("TestSessionKey"))
                {
                    if (yyyyMM.Equals("0000-00"))
                        return Task.FromResult<Dictionary<string, HistoricalDate>?>(new());

                    return Task.FromResult<Dictionary<string, HistoricalDate>?>(null);
                }
                else
                    return Task.FromResult<Dictionary<string, HistoricalDate>?>(null);
            }
        );
        habitHistoryController = new HabitHistoryController(mockHabitHistoryService.Object);
    }

      private void SetValidSessionKey()
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers["Cookie"] = "SessionKey=TestSessionKey";
        habitHistoryController.ControllerContext.HttpContext = httpContext;
    }

    private void SetInvalidSessionKey()
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers["Cookie"] = "SessionKey=TestSessionKeyInvalid";
        habitHistoryController.ControllerContext.HttpContext = httpContext;
    }
    
    [Fact]
    public async Task TestSetHabitCompletion()
    {
        SetValidSessionKey();

        IActionResult result = await habitHistoryController.SetHabitCompletion(
            new CompleteHabitRequest
            {
                HabitId = "1234",
                Date = "2025-05-22",
                Completed = true
            }
        );
        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task TestSetHabitCompletionInvalid()
    {
        SetInvalidSessionKey();

        IActionResult result = await habitHistoryController.SetHabitCompletion(
            new CompleteHabitRequest
            {
                HabitId = "1234",
                Date = "2025-05-22",
                Completed = true
            }
        );
        Assert.IsType<NotFoundResult>(result);

        SetValidSessionKey();

        result = await habitHistoryController.SetHabitCompletion(
            new CompleteHabitRequest
            {
                HabitId = "1233",
                Date = "2025-05-22",
                Completed = true
            }
        );
        Assert.IsType<NotFoundResult>(result);

        result = await habitHistoryController.SetHabitCompletion(
            new CompleteHabitRequest
            {
                HabitId = "1234",
                Date = "2025-05-21",
                Completed = true
            }
        );
        Assert.IsType<NotFoundResult>(result);

    }

    [Fact]
    public async Task TestGetHabitHistoryByMonth()
    {
        SetValidSessionKey();
        IActionResult result = await habitHistoryController.GetHabitHistoryByMonth("0000-00");
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.IsType<Dictionary<string, HistoricalDate>>(okResult.Value);
    }

    [Fact]
    public async Task TestGetHabitHistoryByMonthInvalid()
    {
        SetValidSessionKey();
        IActionResult result = await habitHistoryController.GetHabitHistoryByMonth("0000-01");
        Assert.IsType<NotFoundResult>(result);

        SetInvalidSessionKey();
        result = await habitHistoryController.GetHabitHistoryByMonth("0000-00");
        Assert.IsType<NotFoundResult>(result);
    }
}