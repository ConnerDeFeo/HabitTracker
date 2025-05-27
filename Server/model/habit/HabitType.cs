namespace Server.model.habit;

/// <summary>
/// The different types of habit a user can hold.
/// The types are represented as integers in the db and
/// the front-end.
/// </summary>
public enum HabitType
{
    BINARY, //1
    TIME, //2
    NUMERIC //3
}