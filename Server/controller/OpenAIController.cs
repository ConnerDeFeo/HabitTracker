using Microsoft.AspNetCore.Mvc;
using Server.service.interfaces;

namespace Server.controller;

[Route("openAI")]
[ApiController]
public class OpenAiController(IOpenAIHabitService openAIHabitService) : ControllerBase
{
    private readonly IOpenAIHabitService _openAIHabitService = openAIHabitService;

    [HttpGet]
    public async Task<IActionResult> GetResponse()
    {
        string? sessionKey = Request.Cookies["sessionKey"];
        if (sessionKey is null)
            return Unauthorized();
        string? aiResp = await _openAIHabitService.GetReccomendation(sessionKey);
        if (aiResp is null)
            return StatusCode(429);
        return Ok(aiResp);
    }
}