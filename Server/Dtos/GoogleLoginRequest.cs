namespace Server.dtos;


/// <summary>
/// Servers to hold all data needed for habit completion.
/// date is not fixed to the current date as users may need
/// to retroactivley complete habits they forgot to set as complete
/// </summary>
public class GoogleLoginRequest
{
    public string Jwt { get; set; } = null!;
    public string DeviceId { get; set; } = null!;
}