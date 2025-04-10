namespace Server.controller;
using Microsoft.AspNetCore.Mvc;
using Server.service;


[Route("users")]
[ApiController]
public class UserController(MongoDbService mongoService) : ControllerBase
{
    private readonly MongoDbService _mongoService = mongoService;

    [HttpPost]
    public IActionResult PostUsers([FromBody] string user)
    {
        if (user == null)
        {
            return BadRequest("User is required.");
        }
        if(_mongoService.AddUser(user)){
            return Ok(); 
        }
        return BadRequest("User already exists");
    }
}