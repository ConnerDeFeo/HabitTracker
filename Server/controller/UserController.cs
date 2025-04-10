namespace Server.controller;
using Microsoft.AspNetCore.Mvc;
using Server.service;


[Route("users")]
[ApiController]
public class UserController(MongoDbService mongoService) : ControllerBase
{
    private readonly MongoDbService _mongoService = mongoService;

    [HttpPost]
    public IActionResult PostUsers([FromBody] string username)
    {
        if (username == null)
        {
            return BadRequest("User is required.");
        }
        if(_mongoService.AddUser(username)){
            return Ok(); 
        }
        return BadRequest("User already exists");
    }
}