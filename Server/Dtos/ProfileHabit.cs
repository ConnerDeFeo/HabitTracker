namespace Server.dtos;

public class ProfileHabit
{
    public string Name { get; set; } = null!;

    public string DateCreated { get; set; } = null!;

    public int CurrentStreak { get; set; } = 0;

}