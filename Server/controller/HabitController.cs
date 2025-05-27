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
            if (habits != null)
                return Ok(habits);
        }
        return Unauthorized();

    }

    [HttpPost]
    public async Task<IActionResult> CreateHabit([FromBody] Habit habit)
    {
        var sesionKey = Request.Cookies["sessionKey"];
        if (sesionKey != null)
        {
            Habit? createdHabit = await _habitService.CreateHabit(sesionKey, habit);
            if (createdHabit != null)
                return Ok(createdHabit);
        }
        return Unauthorized();
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteHabit([FromHeader] string habitId)
    {
        var sesionKey = Request.Cookies["sessionKey"];
        if (sesionKey != null)
        {
            bool habitDeleted = await _habitService.DeleteHabit(sesionKey, habitId);
            if (habitDeleted) return Ok();
        }
        return Unauthorized();
    }

    [HttpPut]
    public async Task<IActionResult> EditHabit([FromBody] Habit habit)
    {
        var sesionKey = Request.Cookies["sessionKey"];
        if (sesionKey != null)
        {
            Habit? editedHabit = await _habitService.EditHabit(sesionKey, habit);
            if (editedHabit != null) return Ok(editedHabit);
        }
        return Unauthorized();
    }

    [HttpPost("complete")]
    public async Task<IActionResult> SetHabitCompletion([FromBody] CompleteHabitRequest habitRequest)
    {
        var sesionKey = Request.Cookies["sessionKey"];
        if (sesionKey != null)
        {
            //Note this will be the list of habits that correspond with the date
            bool completed = await _habitService.SetHabitCompletion(sesionKey,habitRequest.Date, habitRequest.Habit, habitRequest.Completed);
            if (completed) return Ok();
        }
        return Unauthorized();
    }

}