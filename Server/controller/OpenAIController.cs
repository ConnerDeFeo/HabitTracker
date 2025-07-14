namespace Server.controller;

using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OpenAI.Interfaces;
using Server.service.concrete;

/// <summary>
/// Controles friend interactions
/// </summary>
[Route("openAi")]
[ApiController]
public class OpenAiController(OpenAiHabitService openAiHabitService) : ControllerBase
{
    private readonly OpenAiHabitService _openAiHabitService = openAiHabitService;

    [HttpGet("{query}")]
    public async Task<IActionResult<string>> GetRecomendation(string query)
    {
        string? sessionKey = Request.Cookies["sessionKey"];
        if (sessionKey is null)
            return Unauthorized();

        string? resp = await _openAiHabitService.GetResponse(sessionKey, query);
        if (resp is not null)
            return Ok(resp);
        return StatusCode(429, "Sorry, I ran out of money for OpenAI requests! :(");
    }
}