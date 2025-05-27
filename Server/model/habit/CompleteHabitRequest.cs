namespace Server.model.habit;

/// <summary>
/// Servers to hold all data needed for habit completion.
/// date is not fixed to the current date as users may need
/// to retroactivley complete habits they forgot to set as complete
/// </summary>
public class CompleteHabitRequest
{
    public string HabitId { get; set; } = null!;
    public string Date { get; set; } = null!;
    public bool Completed { get; set; }
}