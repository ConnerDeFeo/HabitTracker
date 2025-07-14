using MongoDB.Driver;
using OpenAI.Interfaces;
using Server.model.user;
using Server.service.utils;
using OpenAI.ObjectModels;
using OpenAI.ObjectModels.RequestModels;


namespace Server.service.concrete;

/// <summary>
/// Simply checks to see if user exists then fetches the call
/// </summary>
public class OpenAiHabitService(IMongoDatabase database, IOpenAIService openAiService)
{
    private readonly IMongoCollection<User> _users = database.GetCollection<User>("Users");
    private readonly IOpenAIService _openAiService = openAiService;
    public async Task<string?> GetResponse(string sessionKey, string query)
    {
        User? user = await UserUtils.GetUserBySessionKey(sessionKey, _users);
        if (user is null)
            return null;
        
        //Send chat
        var chatRequest = new ChatCompletionCreateRequest
        {
            Model = Models.Gpt_3_5_Turbo,
            Messages =
            [
                ChatMessage.FromSystem("You are looking at this users habit history ie days completed. Give some recomendation to them."),
                ChatMessage.FromUser(query)
            ]
        };

        var result = await _openAiService.ChatCompletion.CreateCompletion(chatRequest);

        return result.Successful
            ? result.Choices.FirstOrDefault()?.Message?.Content
            : $"Error: {result.Error?.Message}";
    }
}