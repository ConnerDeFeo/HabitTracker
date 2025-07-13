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
            {
                Response.Cookies.Append("sessionKey", result.SessionKey, new CookieOptions
                {
                    HttpOnly = true,
                    SameSite = SameSiteMode.None,
                    Expires = DateTimeOffset.UtcNow.AddDays(7),
                    Secure = true,
                });
                return Ok(result.User);
            }
        }
        return Unauthorized();
    }
}