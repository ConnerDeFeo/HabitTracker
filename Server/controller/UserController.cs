namespace Server.controller;

using BCrypt;
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
    public async Task<IActionResult> PostUser([FromBody] User user)
    {
        if (user.Username == null || user.Password ==null)
        {
            return BadRequest("User is required.");
        }
        if(await _mongoService.AddUser(user.Username,BCrypt.Net.BCrypt.HashPassword(user.Password))){
            return Ok(); 
        }
        return BadRequest("User already exists");
    }
}