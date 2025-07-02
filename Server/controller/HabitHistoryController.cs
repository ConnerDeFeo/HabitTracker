namespace Server.controller;

using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Server.Dtos;
using Server.model.habit;
using Server.service.interfaces;

/// <summary>
/// Main controller for dictating habit changes.
/// Logic for how this is done is delegated to the
/// habit service interface. 
/// </summary>
[Route("habitHistory")]
[ApiController]
public class HabitHistoryController(IHabitHistoryService _habitHistoryService) : ControllerBase
{ 
    [HttpPut]
    public async Task<IActionResult> SetHabitCompletion([FromBody] CompleteHabitRequest habitRequest)
    {
        string? sesionKey = Request.Cookies["sessionKey"];
        if (sesionKey != null)
        {
            //Note this will be the list of habits that correspond with the date
            bool changed = await _habitHistoryService.SetHabitCompletion(sesionKey, habitRequest.Date, habitRequest.HabitId, habitRequest.Completed);
            if (changed)
                return Ok();
            return NotFound();
        }
        return Unauthorized();
    }

    [HttpGet("{yyyyMM}")]
    public async Task<IActionResult> GetHabitHistoryByMonth(string yyyyMM)
    {
        string? sesionKey = Request.Cookies["sessionKey"];
        if (sesionKey != null)
        {
            //Note this will be the list of habits that correspond with the date
            Dictionary<string, HistoricalDate>? month = await _habitHistoryService.GetHabitHistoryByMonth(sesionKey, yyyyMM);
            if (month != null)
                return Ok(month);
            return NotFound();
        }
        return Unauthorized();
    }
}