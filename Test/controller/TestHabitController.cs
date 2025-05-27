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
        var MockHabitService = new Mock<IHabitService>();



        MockHabitService
        .Setup(hs => hs.CreateHabit(It.IsAny<string>(), It.IsAny<Habit>()))
        .Returns<string, Habit>((sessionKey, habit) =>
        {
            if (sessionKey.Equals("TestSessionKey"))
                return Task.FromResult<Habit?>(habit);
            else
                return Task.FromResult<Habit?>(null);

        }
        );

        MockHabitService
        .Setup(hs => hs.GetHabits(It.IsAny<string>()))
        .Returns<string>((sessionKey) =>
        {
            if (sessionKey.Equals("TestSessionKey"))
                return Task.FromResult<List<Habit>?>(new List<Habit> { new Habit { Name = "TestHabit" } });
            else
                return Task.FromResult<List<Habit>?>(null);

        }
        );
        habitController = new HabitController(MockHabitService.Object);

        MockHabitService
        .Setup(hs => hs.DeleteHabit(It.IsAny<string>(), It.IsAny<string>()))
        .Returns<string, string>((sessionKey, habitId) =>
        {
            if (sessionKey.Equals("TestSessionKey"))
            {
                if (habitId.Equals("1234" ))
                    return Task.FromResult<bool>(true);
                else return Task.FromResult<bool>(false);
            }
            else
                return Task.FromResult<bool>(false);
        }
        );

        MockHabitService
        .Setup(hs => hs.EditHabit(It.IsAny<string>(), It.IsAny<Habit>()))
        .Returns<string, Habit>((sessionKey, habit) =>
        {
            if (sessionKey.Equals("TestSessionKey"))
            {
                if (habit.Equals(new Habit { Name = "TestHabit", Id = "1234" }))
                    return Task.FromResult<Habit?>(habit);
                else return Task.FromResult<Habit?>(null);
            }
            else
                return Task.FromResult<Habit?>(null);
        }
        );

        MockHabitService
        .Setup(hs => hs.SetHabitCompletion(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()))
        .Returns<string, string, string, bool>((sessionKey, date, habitId, completed) =>
        {
            if (sessionKey.Equals("TestSessionKey"))
            {
                if (habitId.Equals("1234" ) && date.Equals("2025-05-22"))
                    return Task.FromResult<bool>(true);
                else
                    return Task.FromResult<bool>(false);
            }
            else
            {
                return Task.FromResult<bool>(false);
            }
        }
        );
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

        IActionResult result = await habitController.GetHabits();
        var okResult = Assert.IsType<OkObjectResult>(result);
        var habitResult = Assert.IsType<List<Habit>>(okResult.Value);
        Assert.NotEmpty(habitResult);
        Assert.Equal("TestHabit", habitResult[0].Name);
    }

    [Fact]
    public async Task TestGetHabitsInvalid()
    {
        SetInvalidSessionKey();

        IActionResult result = await habitController.GetHabits();
        Assert.IsType<UnauthorizedResult>(result);
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
        Assert.IsType<UnauthorizedResult>(result);

        SetValidSessionKey();
        result = await habitController.DeleteHabit("InValidId");
        Assert.IsType<UnauthorizedResult>(result);
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
        Assert.IsType<UnauthorizedResult>(result);

        SetValidSessionKey();
        result = await habitController.EditHabit(new Habit { Name = "TestHabit", Id = "InvalidId" });
        Assert.IsType<UnauthorizedResult>(result);
    }

    [Fact]
    public async Task TestSetHabitCompletion()
    {
        SetValidSessionKey();

        IActionResult result = await habitController.SetHabitCompletion(
            new CompleteHabitRequest
            {
                HabitId = "1234" ,
                Date = "2025-05-22",
                Completed=true
            }
        );
        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task TestSetHabitCompletionInvalid()
    {
        SetInvalidSessionKey();

        IActionResult result = await habitController.SetHabitCompletion(
            new CompleteHabitRequest
            {
                HabitId = "1234" ,
                Date = "2025-05-22",
                Completed = true
            }
        );
        Assert.IsType<UnauthorizedResult>(result);

        SetValidSessionKey();

        result = await habitController.SetHabitCompletion(
            new CompleteHabitRequest
            {
                HabitId = "1233" ,
                Date = "2025-05-22",
                Completed = true
            }
        );
        Assert.IsType<UnauthorizedResult>(result);

        result = await habitController.SetHabitCompletion(
            new CompleteHabitRequest
            {
                HabitId = "1234" ,
                Date = "2025-05-21",
                Completed = true
            }
        );
        Assert.IsType<UnauthorizedResult>(result);

    }

}