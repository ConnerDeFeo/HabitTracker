namespace Server.controller;

using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Server.model;
using Server.service;


[Route("users")]
[ApiController]
public class UserController(IUserService _userService) : ControllerBase
{
    private readonly IUserService _userService = _userService;

    [HttpGet]
    public async Task<IActionResult> GetUser()
    {
        var cookie = Request.Cookies["sessionKey"];
        if(cookie!=null){
            User? result = await _userService.GetUser(cookie);
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

    [HttpPost("logout")]
    public async Task<IActionResult> Logout(){
        var cookie = Request.Cookies["sessionKey"];
        if(cookie!=null && await _userService.Logout(cookie)){
            return Ok();
        }
        return Unauthorized();
    }
}