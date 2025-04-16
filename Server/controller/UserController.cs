namespace Server.controller;

using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Server.model;
using Server.service;


[Route("users")]
[ApiController]
public class UserController(MongoDbService mongoService) : ControllerBase
{
    private readonly MongoDbService _mongoService = mongoService;

    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] User user)
    {
        if(await _mongoService.CreateUser(user.Username,user.Password)){
            return Ok(); 
        }
        return Conflict("User already exists");
    }

    [HttpGet]
    public async Task<IActionResult> Login([FromBody] User user){
        if(await _mongoService.Login(user.Username, user.Password)){
            return Ok();
        }
        return Unauthorized();

    }
}