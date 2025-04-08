using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Server.Service;


[Route("api/users")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly MongoDbService _mongoService;
    public UserController(MongoDbService mongoService)
    {
        _mongoService = mongoService;
    }
}