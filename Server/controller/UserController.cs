namespace Server.controller;

using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Server.model;
using Server.service;


[Route("users")]
[ApiController]
public class UserController(UserService _userService) : ControllerBase
{
    private readonly UserService _userService = _userService;

    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] User user)
    {
        if(await _userService.CreateUser(user.Username,user.Password)){
            return Ok(); 
        }
        return Conflict("User already exists");
    }

    [Route("login")]
    [HttpPost]
    public async Task<IActionResult> Login([FromBody] User user){
        LoginResult result = await _userService.Login(user.Username,user.Password);
        if(result.Success){
            return Ok(result);
        }
        return Unauthorized();
    }
}