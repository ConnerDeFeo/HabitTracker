namespace Server.controller;

using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Server.model.habit;
using Server.service;

[Route("habits")]
[ApiController]
public class HabitController(IHabitService _habitService) : ControllerBase
{
    private readonly IHabitService _habitService = _habitService;

    [HttpGet]
    public async Task<IActionResult> GetHabits()
    {
        var sesionKey = Request.Cookies["sessionKey"];

        if (sesionKey != null)
        {
            List<Habit>? habits = await _habitService.GetHabits(sesionKey);
            if (habits != null) return Ok(habits);
        }
        return Unauthorized();

    }

    [HttpPost]
    public async Task<IActionResult> CreateHabit([FromBody] Habit habit)
    {
        var sesionKey = Request.Cookies["sessionKey"];
        if (sesionKey != null)
        {
            List<Habit>? habits = await _habitService.CreateHabit(sesionKey, habit);
            if (habits != null) return Ok(habits);
        }
        return Unauthorized();
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteHabit([FromBody] Habit habit)
    {
        var sesionKey = Request.Cookies["sessionKey"];
        if (sesionKey != null)
        {
            List<Habit>? habits = await _habitService.DeleteHabit(sesionKey, habit);
            if (habits != null) return Ok(habits);
        }
        return Unauthorized();
    }

    [HttpPut]
    public async Task<IActionResult> EditHabit([FromBody] Habit habit)
    {
        var sesionKey = Request.Cookies["sessionKey"];
        if (sesionKey != null)
        {
            List<Habit>? habits = await _habitService.EditHabit(sesionKey, habit);
            if (habits != null) return Ok(habits);
        }
        return Unauthorized();
    }

    [HttpPost("complete")]
    public async Task<IActionResult> CompleteHabit([FromBody] CompleteHabitRequest habitRequest)
    {
        var sesionKey = Request.Cookies["sessionKey"];
        if (sesionKey != null)
        {
            
            List<Habit>? habits = await _habitService.CompleteHabit(sesionKey, habitRequest.Habit, habitRequest.Date);
            if (habits != null) return Ok(habits);
        }
        return Unauthorized();
    }

}