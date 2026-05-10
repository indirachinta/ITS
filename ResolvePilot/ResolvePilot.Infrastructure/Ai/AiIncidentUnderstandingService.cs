using System.Text.Json;
using Microsoft.Extensions.AI;
using ResolvePilot.Application.Ai;
using ResolvePilot.Application.Prompts;
using ResolvePilot.Application.Specs;
using ResolvePilot.Domain;
using ResolvePilot.Domain.Ai;

namespace ResolvePilot.Infrastructure.Ai;

public sealed class AiIncidentUnderstandingService(IChatClient chatClient, IPromptBuilder promptBuilder)
    : IAiIncidentUnderstandingService
{
    private const string DecisionSource = "runtime-spec-governed-ai-github-models";

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true
    };

    public async Task<AiIncidentUnderstanding> UnderstandAsync(
        IncidentRequest request,
        RuntimeSpecSet runtimeSpecs,
        CancellationToken cancellationToken = default)
    {
        string prompt = promptBuilder.BuildIncidentUnderstandingPrompt(request, runtimeSpecs);

        ChatResponse response = await chatClient.GetResponseAsync(
            [
                new ChatMessage(ChatRole.System, "Return only valid JSON. You classify and understand incidents. Do not provide final resolution."),
                new ChatMessage(ChatRole.User, prompt)
            ],
            new ChatOptions
            {
                Temperature = 0,
                MaxOutputTokens = 700
            },
            cancellationToken);

        AiIncidentUnderstanding understanding = ParseUnderstanding(response.Text);

        return understanding with
        {
            AffectedService = string.IsNullOrWhiteSpace(understanding.AffectedService)
                ? request.AffectedService
                : understanding.AffectedService,
            Reasoning = understanding.Reasoning.Count == 0
                ? ["AI classified the incident without detailed reasoning."]
                : understanding.Reasoning,
            DecisionSource = string.IsNullOrWhiteSpace(understanding.DecisionSource)
                ? DecisionSource
                : understanding.DecisionSource
        };
    }

    private static AiIncidentUnderstanding ParseUnderstanding(string responseText)
    {
        string json = ExtractJsonObject(responseText);
        AiIncidentUnderstanding? understanding;

        try
        {
            understanding = JsonSerializer.Deserialize<AiIncidentUnderstanding>(json, JsonOptions);
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException("AI response was not valid JSON for the required incident understanding contract.", ex);
        }

        if (understanding is null ||
            string.IsNullOrWhiteSpace(understanding.IncidentIntent) ||
            string.IsNullOrWhiteSpace(understanding.AffectedService))
        {
            throw new InvalidOperationException("AI incident understanding JSON is missing required fields.");
        }

        return understanding;
    }

    private static string ExtractJsonObject(string responseText)
    {
        int start = responseText.IndexOf('{');
        int end = responseText.LastIndexOf('}');

        return start >= 0 && end >= start
            ? responseText[start..(end + 1)]
            : responseText;
    }
}
