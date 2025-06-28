namespace Server.controller;

using Microsoft.AspNetCore.Mvc;
using Server.service.concrete;

/// <summary>
/// Main controller for dictating habit changes.
/// Logic for how this is done is delegated to the
/// habit service interface. 
/// </summary>
[Route("photos")]
[ApiController]
public class PhotoController(PhotoService _photoService) : ControllerBase
{
    private readonly PhotoService _photoService = _photoService;

    [HttpPost]
    public async Task<IActionResult> UploadProfilePhoto(IFormFile file)
    {
        string? sesionKey = Request.Cookies["sessionKey"];
        if (sesionKey != null)
        {
            string? result = await _photoService.UploadProfilePhoto(sesionKey, file);
            if (result is not null)
                return Ok(result);
            return NotFound();
        }
        return Unauthorized();
    }

    [HttpGet]
    public async Task<IActionResult> GetProfilePhoto()
    {
        string? sesionKey = Request.Cookies["sessionKey"];
        if (sesionKey != null)
        {
            string? result = await _photoService.GetProfilePhoto(sesionKey);
            if (result is not null)
                return Ok(result);
            return NotFound();
        }
        return Unauthorized();
    }
}