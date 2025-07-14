using MongoDB.Driver;
using OpenAI.Interfaces;
using Server.model.user;
using Server.service.utils;
using OpenAI.ObjectModels;
using OpenAI.ObjectModels.RequestModels;
using Server.model.habit;
using Server.service.interfaces;


namespace Server.service.concrete;

/// <summary>
/// Simply checks to see if user exists then fetches the call
/// </summary>
public class MongoOpenAIHabitService(IMongoDatabase database, IOpenAIService openAiService): IOpenAIHabitService
{
    private readonly IMongoCollection<User> _users = database.GetCollection<User>("Users");
    private readonly IMongoCollection<HabitCollection> _habitCollections = database.GetCollection<HabitCollection>("HabitCollection");
    private readonly IOpenAIService _openAiService = openAiService;
    public async Task<string?> GetReccomendation(string sessionKey)
    {
        User? user = await UserUtils.GetUserBySessionKey(sessionKey,_users);
        if(user is null)
            return null;
        HashSet<Habit> collection = await HabitUtils.GetActiveHabits(user.Id!,_habitCollections);
        if (collection.Count == 0)
            return "Have an active habit for and AI reccomendation!";

        string aiQuery = string.Join("\n", collection.Select(habit =>
            {
                HashSet<string> daysActive = habit.DaysActive;
                bool containsSaturday = daysActive.Contains("Saturday");
                bool containsSunday = daysActive.Contains("Sunday");

                string days = "";
                if (daysActive.Count == 7)
                    days = "daily";
                else if (daysActive.Count == 2 && containsSaturday && containsSunday)
                    days = "Weekends";
                else if (daysActive.Count == 5 && !containsSaturday && !containsSunday)
                    days = "Weekdays";
                else
                    days = daysActive != null && daysActive.Count != 0
                    ? string.Join(", ", daysActive)
                    : "None";
                

                return $"-{habit.Name}: active {days}";
            }));
        //Send chat
        var chatRequest = new ChatCompletionCreateRequest
        {
            Model = Models.Gpt_3_5_Turbo,
            Messages =
            [
                ChatMessage.FromSystem(@"You're a habit coach.
                Given the user's current habits, suggest one meaningful improvement:
                either add, remove, or change a habit. Be specific and give a brief reason.
                Respond in 2 sentences max."),
                ChatMessage.FromUser(aiQuery)
            ],
            MaxTokens = 100
        };

        var result = await _openAiService.ChatCompletion.CreateCompletion(chatRequest);

        return result.Successful
            ? result.Choices.FirstOrDefault()?.Message?.Content
            : $"All out of AI Reccomendations! (I'm broke and can afford more AI API requests)";
    }
}