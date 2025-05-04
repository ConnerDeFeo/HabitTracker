namespace Server.controller;

using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Server.model;
using Server.service;

[Route("habits")]
[ApiController]
public class HabitController(IHabitService _habitService) : ControllerBase
{
    private readonly IHabitService _habitService = _habitService;

    [HttpGet]
    public async Task<IActionResult> GetHabits(){
        var sesionKey = Request.Cookies["sessionKey"];

        if(sesionKey!=null){
            List<Habit>? habits = await _habitService.GetHabits(sesionKey);
            if(habits!=null) return Ok(habits);
        }
        return Unauthorized();

    }
}