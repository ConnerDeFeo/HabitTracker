
namespace Server.dtos;

/// <summary>
/// Basic data regarding a user including their current active habits and current monthly habits
/// </summary>
public class Profile
{
    public string Username { get; set; } = null!;
    public string Id { get; set; } = null!;
    public string DateCreated { get; set; } = null!;
    public List<ProfileHabit> CurrentHabits { get; set; } = [];

    public Dictionary<string, bool> CurrentMonthHabitsCompleted { get; set; } = [];
}