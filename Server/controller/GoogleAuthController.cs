namespace Server.controller;

using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Server.dtos;
using Server.service.interfaces;

/// <summary>
/// Controles friend interactions
/// </summary>
[Route("googleAuth")]
[ApiController]
public class GoogleAuthController(IGoogleAuthService googleAuthService) : ControllerBase
{
    private readonly IGoogleAuthService _googleAuthService = googleAuthService;

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] GoogleLoginRequest request)
    { 
        string? sesionKey = Request.Cookies["sessionKey"];

        if (sesionKey != null)
        {
            LoginResult result = await _googleAuthService.Login(request.Jwt, request.DeviceId);
            if (result.SessionKey != "")
                return Ok(result);
        }
        return Unauthorized();
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateUser([FromBody] GoogleLoginRequest request)
    {
        string? sesionKey = Request.Cookies["sessionKey"];

        if (sesionKey != null)
        {
            LoginResult result = await _googleAuthService.CreateUser(request.Jwt, request.DeviceId);
            if (result.SessionKey != "")
                return Ok(result);
        }
        return Unauthorized($"sessionKey invalid, {sesionKey} is null");

    }
}