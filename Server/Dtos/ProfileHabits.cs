
namespace Server.dtos;

/// <summary>
/// Basic data regarding a user including their current active habits and current monthly habits
/// </summary>
public class ProfileHabits
{

    public List<ProfileHabit> CurrentHabits { get; set; } = [];

    public Dictionary<string, bool> CurrentMonthHabitsCompleted { get; set; } = [];
}