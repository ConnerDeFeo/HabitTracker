namespace Server.controller;

using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Server.dtos;
using Server.service.interfaces;

/// <summary>
/// Retreves data for other people profiles
/// </summary>
[Route("socialData")]
[ApiController]
public class SocialDataController(ISocialDataService socialDataService) : ControllerBase
{
    private readonly ISocialDataService _socialDataService = socialDataService;

    [HttpGet]
    public async Task<IActionResult> GetFriends()
    {
        string? sesionKey = Request.Cookies["sessionKey"];

        if (sesionKey != null)
        {
            Dictionary<string, string>? friends = await _socialDataService.GetFriends(sesionKey);
            if (friends != null)
                return Ok(friends);
            return NotFound();
        }
        return Unauthorized();
    }

    [HttpGet("profile/{username}")]
    public async Task<IActionResult> GetProfile(string username)
    {
        string? sesionKey = Request.Cookies["sessionKey"];

        if (sesionKey != null)
        {
            Profile? profile = await _socialDataService.GetProfile(sesionKey, username);
            if (profile != null)
                return Ok(profile);
            return NotFound();
        }
        return Unauthorized();
    }

    [HttpGet("find/{phrase}")]
    public async Task<IActionResult> FindUser(string phrase)
    {
        string? sesionKey = Request.Cookies["sessionKey"];

        if (sesionKey != null)
        {
            Dictionary<string, string>? users = await _socialDataService.FindUser(sesionKey, phrase);
            if (users is not null)
                return Ok(users);
            return NotFound();
        }
        return Unauthorized();
    }

    [HttpGet("random")]
    public async Task<IActionResult> GetRandomUsers()
    {
        string? sesionKey = Request.Cookies["sessionKey"];

        if (sesionKey != null)
        {
            Dictionary<string, string>? users = await _socialDataService.GetRandomUsers(sesionKey);
            if (users is not null)
                return Ok(users);
            return NotFound();
        }
        return Unauthorized();
    }
}