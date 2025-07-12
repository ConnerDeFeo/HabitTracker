namespace Server.dtos;

public class LoginRequest
{
    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;
    public string DeviceId { get; set; } = null!;

    public string? Email { get; set; }
}