using Microsoft.AspNetCore.Mvc;
using Server.model.habit;
using Server.service;

namespace Server.controller;

[Route("habitStatistics")]
[ApiController]
public class HabitStatisticController(IHabitStatisticService _statisticService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetHistoricalData([FromBody]Habit habit)
    { 
        string? sesionKey = Request.Cookies["sessionKey"];
        if (sesionKey != null)
        {
            //Note this will be the list of habits that correspond with the date
            HistoricalData? data = await _statisticService.GetHistoricalData(sesionKey, habit);
            if (data is not null)
                return Ok(data);
            return NotFound();
        }
        return Unauthorized();
    }
}