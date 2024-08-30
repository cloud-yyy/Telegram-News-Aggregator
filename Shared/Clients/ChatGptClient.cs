using Entities.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OpenAI.Chat;
using Shared.Params;

namespace Shared.Clients;

public class ChatGptClient
{
    private readonly ChatGPTParams _modelParameters;
    private readonly ChatClient _client;

    public ChatClient Client => _client;
    public ChatGPTParams Params => _modelParameters;

    public ChatGptClient(ILogger<ChatGptClient> logger, IConfiguration configuration)
    {
        var paramsSection = configuration.GetSection("OpenAIParams");
        var token = Environment.GetEnvironmentVariable("openai_token");

        if (paramsSection == null)
            throw new ConfigurationNotFoundException("OpenAIParams");

        if (token == null)
            throw new EnviromentVariableNotFoundException("openai_token");

        _modelParameters = new
        (
            token: token,
            modelVersion: paramsSection["ModelVersion"]!,
            summarizeSinglePrompt: paramsSection["SummarizeSinglePrompt"]!,
            summarizeManyPrompt: paramsSection["SummarizeManyPrompt"]!,
            extractKeywordsPrompt: paramsSection["ExtractKeywordsPrompt"]!,
            comparePrompt: paramsSection["ComparePrompt"]!,
            titleSummarySeparator: paramsSection["TitleSummarySeparator"]!,
            messagesForSummarizeSeparator: paramsSection["MessagesForSummarizeSeparator"]!
        );

        try
        {
            _client = new(_modelParameters.ModelVersion, token);
            logger.LogInformation("ChatGPT client: logged in.");
        }
        catch (Exception ex)
        {
            logger.LogError($"ChatGPT client: login failed: {ex}");
        }
    }

    public static string TrimJsonResponse(string response, char startsWith = '{')
    {
        response = response.Trim('`');
        var startIndex = 0;

        while (response[startIndex] != startsWith)
            startIndex++;

        return response[startIndex..];
    }
}
