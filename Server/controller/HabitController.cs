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

    [HttpGet]
    public async Task<IActionResult> GetExistingHabits()
    { 
        string? sesionKey = Request.Cookies["sessionKey"];

        if (sesionKey != null)
        {
            Dictionary<string,List<Habit>>? habits = await _habitService.GetExistingHabits(sesionKey);
            if (habits != null)
                return Ok(habits);
            return NotFound();
        }
        return Unauthorized();
    }

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
        return Unauthorized($"sessionKey invalid, {sesionKey} is null");

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

    [HttpDelete("deactivate/{habitId}")]
    public async Task<IActionResult> DeactivateHabit(string habitId)
    {
        string? sesionKey = Request.Cookies["sessionKey"];
        if (sesionKey != null)
        {
            bool deactivated = await _habitService.DeactivateHabit(sesionKey, habitId);
            if (deactivated)
                return Ok();
            return NotFound();
        }
        return Unauthorized();
    }

    [HttpPost("reactivate/{habitId}")]
    public async Task<IActionResult> ReactivateHabit(string habitId)
    { 
        string? sesionKey = Request.Cookies["sessionKey"];
        if (sesionKey != null)
        {
            bool reactivated = await _habitService.ReactivateHabit(sesionKey, habitId);
            if (reactivated)
                return Ok();
            return NotFound();
        }
        return Unauthorized();
    }
}