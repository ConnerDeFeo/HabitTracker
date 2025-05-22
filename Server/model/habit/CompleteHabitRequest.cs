namespace Server.model.habit;

public class CompleteHabitRequest
{
    public Habit Habit { get; set; } = null!;
    public string Date { get; set; } = null!;
    public bool Completed { get; set; }
}