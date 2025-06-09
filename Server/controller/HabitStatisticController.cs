using Microsoft.AspNetCore.Mvc;
using Server.model.habit;
using Server.service;

namespace Server.controller;

[Route("habitStatistics")]
[ApiController]
public class HabitStatisticController(IHabitStatisticService _statisticService) : ControllerBase
{
    [HttpGet("{habitId}")]
    public async Task<IActionResult> GetHistoricalData(string habitId)
    { 
        string? sesionKey = Request.Cookies["sessionKey"];
        if (sesionKey != null)
        {
            //Note this will be the list of habits that correspond with the date
            HistoricalData? data = await _statisticService.GetHistoricalData(sesionKey, habitId);
            if (data is not null)
                return Ok(data);
            return NotFound();
        }
        return Unauthorized();
    }
}