namespace Server.controller;

using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Server.model.habit;
using Server.service;

/// <summary>
/// Main controller for dictating habit changes.
/// Logic for how this is done is delegated to the
/// habit service interface. 
/// </summary>
[Route("habits")]
[ApiController]
public class HabitController(IHabitService _habitService) : ControllerBase
{
    private readonly IHabitService _habitService = _habitService;

    [HttpGet("{date}")]
    public async Task<IActionResult> GetHabits(string date)
    {
        string? sesionKey = Request.Cookies["sessionKey"];

        if (sesionKey != null)
        {
            List<Habit>? habits = await _habitService.GetHabits(sesionKey, date);
            if (habits != null)
                return Ok(habits);
            return NotFound();
        }
        return Unauthorized();

    }

    [HttpPost]
    public async Task<IActionResult> CreateHabit([FromBody] Habit habit)
    {
        string? sesionKey = Request.Cookies["sessionKey"];
        if (sesionKey != null)
        {
            Habit? createdHabit = await _habitService.CreateHabit(sesionKey, habit);
            if (createdHabit != null)
                return Ok(createdHabit);
            return Conflict();
        }
        return Unauthorized();
    }

    [HttpDelete("{habitId}")]
    public async Task<IActionResult> DeleteHabit(string habitId)
    {
        string? sesionKey = Request.Cookies["sessionKey"];
        if (sesionKey != null)
        {
            bool habitDeleted = await _habitService.DeleteHabit(sesionKey, habitId);
            if (habitDeleted)
                return Ok();
            return NotFound();
        }
        return Unauthorized();
    }

    [HttpPut]
    public async Task<IActionResult> EditHabit([FromBody] Habit habit)
    {
        string? sesionKey = Request.Cookies["sessionKey"];
        if (sesionKey != null)
        {
            Habit? editedHabit = await _habitService.EditHabit(sesionKey, habit);
            if (editedHabit != null)
                return Ok(editedHabit);
            return NotFound();
        }
        return Unauthorized();
    }

    [HttpPut("habitCompletion")]
    public async Task<IActionResult> SetHabitCompletion([FromBody] CompleteHabitRequest habitRequest)
    {
        string? sesionKey = Request.Cookies["sessionKey"];
        if (sesionKey != null)
        {
            //Note this will be the list of habits that correspond with the date
            bool changed = await _habitService.SetHabitCompletion(sesionKey, habitRequest.Date, habitRequest.HabitId, habitRequest.Completed);
            if (changed)
                return Ok();
            return NotFound();
        }
        return Unauthorized();
    }

    [HttpGet("month/{yyyyMM}")]
    public async Task<IActionResult> GetHabitHistoryByMonth(string yyyyMM)
    {
        string? sesionKey = Request.Cookies["sessionKey"];
        if (sesionKey != null)
        {
            //Note this will be the list of habits that correspond with the date
            Dictionary<string, HistoricalDate>? month = await _habitService.GetHabitHistoryByMonth(sesionKey, yyyyMM);
            if (month != null)
                return Ok(month);
            return NotFound();
        }
        return Unauthorized();
    }

}