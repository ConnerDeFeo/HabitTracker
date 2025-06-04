namespace Test.controller;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using Microsoft.AspNetCore.Http;
using Server.service;
using Server.controller;
using Server.model.user;
using Server.model.habit;
using Moq;

public class TestHabitController
{
    HabitController habitController;

    public TestHabitController()
    {
        var mockHabitService = new Mock<IHabitService>();

        mockHabitService
        .Setup(hs => hs.CreateHabit(It.IsAny<string>(), It.IsAny<Habit>()))
        .Returns<string, Habit>((sessionKey, habit) =>
        {
            if (sessionKey.Equals("TestSessionKey"))
                return Task.FromResult<Habit?>(habit);
            return Task.FromResult<Habit?>(null);

        }
        );

        mockHabitService
        .Setup(hs => hs.GetHabits(It.IsAny<string>(), It.IsAny<string>()))
        .Returns<string, string>((sessionKey, date) =>
        {
            if (sessionKey.Equals("TestSessionKey") && date.Equals("0000-00-00"))
                return Task.FromResult<List<Habit>?>(new List<Habit> { new Habit { Name = "TestHabit" } });
            return Task.FromResult<List<Habit>?>(null);

        }
        );

        mockHabitService
        .Setup(hs => hs.DeleteHabit(It.IsAny<string>(), It.IsAny<string>()))
        .Returns<string, string>((sessionKey, habitId) =>
        {
            if (sessionKey.Equals("TestSessionKey"))
            {
                if (habitId.Equals("1234"))
                    return Task.FromResult<bool>(true);
                return Task.FromResult<bool>(false);
            }
            else
                return Task.FromResult<bool>(false);
        }
        );

        mockHabitService
        .Setup(hs => hs.EditHabit(It.IsAny<string>(), It.IsAny<Habit>()))
        .Returns<string, Habit>((sessionKey, habit) =>
        {
            if (sessionKey.Equals("TestSessionKey"))
            {
                if (habit.Id!.Equals("1234"))
                    return Task.FromResult<Habit?>(habit);
                else
                    return Task.FromResult<Habit?>(null);
            }
            else
                return Task.FromResult<Habit?>(null);
        }
        );

        habitController = new HabitController(mockHabitService.Object);
    }

    private void SetValidSessionKey()
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers["Cookie"] = "SessionKey=TestSessionKey";
        habitController.ControllerContext.HttpContext = httpContext;
    }

    private void SetInvalidSessionKey()
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers["Cookie"] = "SessionKey=TestSessionKeyInvalid";
        habitController.ControllerContext.HttpContext = httpContext;
    }

    [Fact]
    public async Task TestGetHabits()
    {
        SetValidSessionKey();

        IActionResult result = await habitController.GetHabits("0000-00-00");
        var okResult = Assert.IsType<OkObjectResult>(result);
        var habitResult = Assert.IsType<List<Habit>>(okResult.Value);
        Assert.NotEmpty(habitResult);
        Assert.Equal("TestHabit", habitResult[0].Name);
    }

    [Fact]
    public async Task TestGetHabitsInvalid()
    {
        SetInvalidSessionKey();

        IActionResult result = await habitController.GetHabits("0000-00-00");
        Assert.IsType<NotFoundResult>(result);

        SetValidSessionKey();
        result = await habitController.GetHabits("0000-00-01");
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task TestCreateHabit()
    {
        SetValidSessionKey();

        IActionResult result = await habitController.CreateHabit(new Habit { Name = "TestHabit" });
        var okResult = Assert.IsType<OkObjectResult>(result);
        var habitResult = Assert.IsType<Habit>(okResult.Value);
        Assert.Equal("TestHabit", habitResult.Name);
    }

    [Fact]
    public async Task TestDeleteHabit()
    {
        SetValidSessionKey();

        IActionResult result = await habitController.DeleteHabit("1234");
        Assert.IsType<OkResult>(result);
    }
    [Fact]
    public async Task TestDeleteHabitInvalid()
    {
        SetInvalidSessionKey();

        IActionResult result = await habitController.DeleteHabit("1234");
        Assert.IsType<NotFoundResult>(result);

        SetValidSessionKey();
        result = await habitController.DeleteHabit("InValidId");
        Assert.IsType<NotFoundResult>(result);
    }
    [Fact]
    public async Task TestEditHabit()
    {
        SetValidSessionKey();

        IActionResult result = await habitController.EditHabit(new Habit { Name = "TestHabit", Id = "1234" });
        var okResult = Assert.IsType<OkObjectResult>(result);
        var habitResult = Assert.IsType<Habit>(okResult.Value);
        Assert.Equal("TestHabit", habitResult.Name);
    }
    [Fact]
    public async Task TestEditHabitInvalid()
    {
        SetInvalidSessionKey();

        IActionResult result = await habitController.EditHabit(new Habit { Name = "TestHabit", Id = "1234" });
        Assert.IsType<NotFoundResult>(result);

        SetValidSessionKey();
        result = await habitController.EditHabit(new Habit { Name = "TestHabit", Id = "InvalidId" });
        Assert.IsType<NotFoundResult>(result);
    }
}