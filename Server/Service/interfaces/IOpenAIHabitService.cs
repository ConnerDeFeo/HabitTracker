namespace Server.service.interfaces;
public interface IOpenAIHabitService
{
    Task<string?> GetReccomendation(string sessionKey);
}