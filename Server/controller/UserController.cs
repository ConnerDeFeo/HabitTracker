namespace Server.controller;

using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Server.dtos;
using Server.model.user;
using Server.service.interfaces;

/// <summary>
/// Handles user requests dealing with user profiles specifically.
/// This does not include and data regarding habits. Logic is delegated
/// to a concrete implementation of the userService interface.
/// </summary>
/// <param name="_userService"></param>
[Route("users")]
[ApiController]
public class UserController(IUserService _userService) : ControllerBase
{
    private readonly IUserService _userService = _userService;

    /// <summary>
    /// Gets a user based on session key that was received
    /// </summary>
    /// <returns>User (200) if found, 401 else</returns>
    [HttpGet]
    public async Task<IActionResult> GetUser()
    {
        var sesionKey = Request.Cookies["sessionKey"];
        if (sesionKey != null)
        {
            UserDto? result = await _userService.GetUser(sesionKey);

            if (result != null)
                return Ok(result);
        }
        return Unauthorized();
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] LoginRequest request)
    {
        LoginResult result = await _userService.CreateUser(request);
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
        return Conflict(result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        LoginResult result = await _userService.Login(request);
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
        return Unauthorized(result);
    }

    /// <summary>
    /// Logs user out based on session key
    /// </summary>
    /// <returns>200 if succesful logout, 401 else</returns>
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var sesionKey = Request.Cookies["sessionKey"];
        if (sesionKey != null && await _userService.Logout(sesionKey))
            return Ok();
        return Unauthorized();
    }

    [HttpGet("profile")]
    public async Task<IActionResult> GetUserProfile()
    {
        var sesionKey = Request.Cookies["sessionKey"];
        if (sesionKey != null)
        {
            Profile? result = await _userService.GetProfile(sesionKey);

            if (result != null)
                return Ok(result);
        }
        return Unauthorized();
    }

    [HttpPut("{username}")]
    public async Task<IActionResult> ChangeUsername(string username)
    { 
        var sesionKey = Request.Cookies["sessionKey"];
        if (sesionKey != null)
        {
            bool changed = await _userService.ChangeUsername(sesionKey, username);
            if (changed)
                return Ok();
            return Conflict();
        }
        return Unauthorized();
    }
}