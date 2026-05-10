using System.Text.Json;
using IncidentTools.Core;
using Microsoft.Extensions.AI;
using ResolvePilot.Application.Ai;
using ResolvePilot.Application.Prompts;
using ResolvePilot.Application.Specs;
using ResolvePilot.Domain;
using ResolvePilot.Domain.Ai;
using ResolvePilot.Domain.ToolCortex;

namespace ResolvePilot.Infrastructure.Ai;

public sealed class AiResolutionService(IChatClient chatClient, IPromptBuilder promptBuilder) : IAiResolutionService
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true
    };

    public async Task<AiFinalResolution> ResolveAsync(
        IncidentRequest request,
        AiIncidentUnderstanding incidentUnderstanding,
        ToolCortexResponse toolCortexResponse,
        IReadOnlyList<ToolExecutionResult> toolExecutionResults,
        RuntimeSpecSet runtimeSpecs,
        CancellationToken cancellationToken = default)
    {
        string prompt = promptBuilder.BuildFinalResolutionPrompt(
            request,
            incidentUnderstanding,
            toolCortexResponse,
            toolExecutionResults,
            runtimeSpecs);

        ChatResponse response = await chatClient.GetResponseAsync(
            [
                new ChatMessage(ChatRole.System, "Return only valid JSON. Use only provided tool evidence and do not invent facts."),
                new ChatMessage(ChatRole.User, prompt)
            ],
            new ChatOptions
            {
                Temperature = 0,
                MaxOutputTokens = 900
            },
            cancellationToken);

        AiFinalResolution finalResolution = ParseResolution(response.Text);

        return finalResolution with
        {
            Reasoning = finalResolution.Reasoning.Count == 0
                ? ["AI produced a final evidence-based resolution without detailed reasoning."]
                : finalResolution.Reasoning
        };
    }

    private static AiFinalResolution ParseResolution(string responseText)
    {
        string json = ExtractJsonObject(responseText);
        AiFinalResolution? finalResolution;

        try
        {
            finalResolution = JsonSerializer.Deserialize<AiFinalResolution>(json, JsonOptions);
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException("AI response was not valid JSON for the required final resolution contract.", ex);
        }

        if (finalResolution is null ||
            string.IsNullOrWhiteSpace(finalResolution.ResolutionSummary) ||
            string.IsNullOrWhiteSpace(finalResolution.RecommendedAction))
        {
            throw new InvalidOperationException("AI final resolution JSON is missing required fields.");
        }

        return finalResolution;
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
