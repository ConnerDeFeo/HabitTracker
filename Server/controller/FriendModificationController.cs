namespace Server.controller;

using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Server.service.interfaces;

/// <summary>
/// Controles friend interactions
/// </summary>
[Route("friendModification")]
[ApiController]
public class FriendModificationController(IFriendModificationService friendModificationService) : ControllerBase
{
    private readonly IFriendModificationService _friendModificationService = friendModificationService;

    [HttpPut("{friendUsername}")]
    public async Task<IActionResult> SendFriendRequest(string friendUsername)
    {
        string? sesionKey = Request.Cookies["sessionKey"];
        if (sesionKey != null)
        {
            bool sent = await friendModificationService.SendFriendRequest(sesionKey, friendUsername);
            if (sent)
                return Ok();
            return NotFound();
        }
        return Unauthorized();
    }

    [HttpDelete("{friendUsername}")]
    public async Task<IActionResult> UnSendFriendRequest(string friendUsername)
    {
        string? sesionKey = Request.Cookies["sessionKey"];

        if (sesionKey != null)
        {
            bool unsent = await friendModificationService.UnSendFriendRequest(sesionKey, friendUsername);
            if (unsent)
                return Ok();
            return NotFound();
        }
        return Unauthorized();
    }

    [HttpPost("{friendUsername}")]
    public async Task<IActionResult> AcceptFriendRequest(string friendUsername)
    {
        string? sesionKey = Request.Cookies["sessionKey"];

        if (sesionKey != null)
        {
            Dictionary<string, string>? newFriends = await friendModificationService.AcceptFriendRequest(sesionKey, friendUsername);
            if (newFriends is not null)
                return Ok(newFriends);
            return NotFound();
        }
        return Unauthorized();
    }

    [HttpDelete("reject/{friendUsername}")]
    public async Task<IActionResult> RejectFriendRequest(string friendUsername)
    {
        string? sesionKey = Request.Cookies["sessionKey"];

        if (sesionKey != null)
        {
            bool rejected = await friendModificationService.RejectFriendRequest(sesionKey, friendUsername);
            if (rejected)
                return Ok();
            return NotFound();
        }
        return Unauthorized();
    }

    [HttpDelete("remove/{friendUsername}")]
    public async Task<IActionResult> RemoveFriend(string friendUsername)
    {
        string? sesionKey = Request.Cookies["sessionKey"];

        if (sesionKey != null)
        {
            Dictionary<string, string>? newFriends = await friendModificationService.RemoveFriend(sesionKey, friendUsername);
            if (newFriends is not null)
                return Ok(newFriends);
            return NotFound();
        }
        return Unauthorized();
    }
}