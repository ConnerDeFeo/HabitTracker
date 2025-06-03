namespace Test.controller;

public class TestHabitHistoryController
{
    HabitController habitController;

    public TestHabitController()
    {

    }

    
    [Fact]
    public async Task TestSetHabitCompletion()
    {
        SetValidSessionKey();

        IActionResult result = await habitController.SetHabitCompletion(
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

        IActionResult result = await habitController.SetHabitCompletion(
            new CompleteHabitRequest
            {
                HabitId = "1234",
                Date = "2025-05-22",
                Completed = true
            }
        );
        Assert.IsType<NotFoundResult>(result);

        SetValidSessionKey();

        result = await habitController.SetHabitCompletion(
            new CompleteHabitRequest
            {
                HabitId = "1233",
                Date = "2025-05-22",
                Completed = true
            }
        );
        Assert.IsType<NotFoundResult>(result);

        result = await habitController.SetHabitCompletion(
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
        IActionResult result = await habitController.GetHabitHistoryByMonth("0000-00");
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.IsType<Dictionary<string, HistoricalDate>>(okResult.Value);
    }

    [Fact]
    public async Task TestGetHabitHistoryByMonthInvalid()
    {
        SetValidSessionKey();
        IActionResult result = await habitController.GetHabitHistoryByMonth("0000-01");
        Assert.IsType<NotFoundResult>(result);

        SetInvalidSessionKey();
        result = await habitController.GetHabitHistoryByMonth("0000-00");
        Assert.IsType<NotFoundResult>(result);
    }
}