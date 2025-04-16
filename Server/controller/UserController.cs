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
    public async Task<IActionResult> PostUser([FromBody] User user)
    {
        if(await _mongoService.AddUser(user.Username,user.Password)){
            return Ok(); 
        }
        return Conflict("User already exists");
    }
}