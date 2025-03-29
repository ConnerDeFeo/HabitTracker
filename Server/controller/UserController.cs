using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

[Route("api/users")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly MongoDbService _mongoService;
    private readonly IMongoCollection<User> _users;

    public UserController(MongoDbService mongoService)
    {
        _mongoService = mongoService;
        _users = _mongoService.GetCollection<User>("Users");
    }

    [HttpGet]
    public async Task<IActionResult> GetUsers()
    {
        var users = await _users.Find(_ => true).ToListAsync();
        return Ok(users);
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] User user)
    {
        await _users.InsertOneAsync(user);
        return CreatedAtAction(nameof(GetUsers), new { id = user.Id }, user);
    }
}