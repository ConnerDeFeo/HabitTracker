namespace Server.controller;

using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Server.dtos;
using Server.model.habit;
using Server.service.interfaces;

/// <summary>
/// Main controller for dictating habit changes.
/// Logic for how this is done is delegated to the
/// habit service interface. 
/// </summary>
[Route("friends")]
[ApiController]
public class FriendController(IFriendService friendService) : ControllerBase
{
    private readonly IFriendService _friendService = friendService;

    [HttpGet]
    public async Task<IActionResult> GetFriends()
    {
        string? sesionKey = Request.Cookies["sessionKey"];

        if (sesionKey != null)
        {
            Dictionary<string, string?>? friends = await _friendService.GetFriends(sesionKey);
            if (friends != null)
                return Ok(friends);
            return NotFound();
        }
        return Unauthorized();
    }

    [HttpGet("profile/{friendUsername}")]
    public async Task<IActionResult> GetFriendsProfile(string friendUsername)
    {
        string? sesionKey = Request.Cookies["sessionKey"];

        if (sesionKey != null)
        {
            ProfileHabits? profileHabits = await _friendService.GetFriendProfile(sesionKey, friendUsername);
            if (profileHabits != null)
                return Ok(profileHabits);
            return NotFound();
        }
        return Unauthorized();
    }

    [HttpPut]
    public async Task<IActionResult> SendFriendRequest([FromBody] string friendUsername)
    {
        string? sesionKey = Request.Cookies["sessionKey"];

        if (sesionKey != null)
        {
            bool sent = await _friendService.SendFriendRequest(sesionKey, friendUsername);
            if (sent)
                return Ok();
            return NotFound();
        }
        return Unauthorized();
    }

    [HttpDelete]
    public async Task<IActionResult> UnSendFriendRequest([FromBody] string friendUsername)
    {
        string? sesionKey = Request.Cookies["sessionKey"];

        if (sesionKey != null)
        {
            bool unsent = await _friendService.UnSendFriendRequest(sesionKey, friendUsername);
            if (unsent)
                return Ok();
            return NotFound();
        }
        return Unauthorized();
    }

    [HttpPost]
    public async Task<IActionResult> AcceptFriendRequest([FromBody] string friendUsername)
    {
        string? sesionKey = Request.Cookies["sessionKey"];

        if (sesionKey != null)
        {
            Dictionary<string, string?>? newFriends = await _friendService.AcceptFriendRequest(sesionKey, friendUsername);
            if (newFriends is not null)
                return Ok(newFriends);
            return NotFound();
        }
        return Unauthorized();
    }

    [HttpDelete("Reject")]
    public async Task<IActionResult> RejectFriendRequest([FromBody] string friendUsername)
    {
        string? sesionKey = Request.Cookies["sessionKey"];

        if (sesionKey != null)
        {
            bool rejected = await _friendService.RejectFriendRequest(sesionKey, friendUsername);
            if (rejected)
                return Ok();
            return NotFound();
        }
        return Unauthorized();
    }

    [HttpDelete("remove")]
    public async Task<IActionResult> RemoveFriend([FromBody] string friendUsername)
    {
        string? sesionKey = Request.Cookies["sessionKey"];

        if (sesionKey != null)
        {
            Dictionary<string, string?>? newFriends = await _friendService.RemoveFriend(sesionKey, friendUsername);
            if (newFriends is not null)
                return Ok(newFriends);
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
            Dictionary<string, string?>? users = await _friendService.FindUser(sesionKey, phrase);
            if (users is not null)
                return Ok(users);
            return NotFound();
        }
        return Unauthorized();
    }
}