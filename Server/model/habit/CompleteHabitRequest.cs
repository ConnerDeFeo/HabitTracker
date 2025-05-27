namespace Server.model.habit;

public class CompleteHabitRequest
{
    public string HabitId { get; set; } = null!;
    public string Date { get; set; } = null!;
    public bool Completed { get; set; }
}