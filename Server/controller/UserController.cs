namespace Server.controller;

using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Server.model;
using Server.service;

/// <summary>
/// Handles user requests dealing with user profiles specifically.
/// This does not include and data regarding habits. All logic is delegated
/// to the userService class and this class simply routes and returns
/// communications between the front and back end service classes.
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
        var cookie = Request.Cookies["sessionKey"];
        if(cookie!=null){
            UserDto? result = await _userService.GetUser(cookie);
            if(result!=null) return Ok(result);
        }
        return Unauthorized();
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] User user)
    {
        LoginResult result = await _userService.CreateUser(user.Username,user.Password);
        if(result.Success){
            return Ok(result); 
        }
        return Conflict(new LoginResult{Success=false});
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] User user){
        LoginResult result = await _userService.Login(user.Username,user.Password);
        if(result.Success){
            return Ok(result);
        }
        return Unauthorized(new LoginResult{Success=false});
    }

    /// <summary>
    /// Logs user out based on session key
    /// </summary>
    /// <returns>200 if succesful logout, 401 else</returns>
    [HttpPost("logout")]
    public async Task<IActionResult> Logout(){
        var cookie = Request.Cookies["sessionKey"];
        if(cookie!=null && await _userService.Logout(cookie)){
            return Ok();
        }
        return Unauthorized();
    }
}