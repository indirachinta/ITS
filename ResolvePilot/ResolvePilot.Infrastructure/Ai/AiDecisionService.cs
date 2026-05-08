using System.Text.Json;
using Microsoft.Extensions.AI;
using ResolvePilot.Application.Ai;
using ResolvePilot.Application.Prompts;
using ResolvePilot.Application.Specs;
using ResolvePilot.Domain;
using ResolvePilot.Domain.Ai;

namespace ResolvePilot.Infrastructure.Ai;

public sealed class AiDecisionService(IChatClient chatClient, IPromptBuilder promptBuilder) : IAiDecisionService
{
    private const string DecisionSource = "runtime-spec-governed-ai-github-models";

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true
    };

    public async Task<AiIncidentDecision> DecideAsync(
        IncidentRequest request,
        RuntimeSpecSet runtimeSpecs,
        CancellationToken cancellationToken = default)
    {
        string prompt = promptBuilder.Build(request, runtimeSpecs);

        ChatResponse response = await chatClient.GetResponseAsync(
            [
                new ChatMessage(ChatRole.System, "Return only valid JSON. You are a deterministic incident decision service."),
                new ChatMessage(ChatRole.User, prompt)
            ],
            new ChatOptions
            {
                Temperature = 0,
                MaxOutputTokens = 900
            },
            cancellationToken);

        AiIncidentDecision decision = ParseDecision(response.Text);

        return decision with
        {
            AffectedService = string.IsNullOrWhiteSpace(decision.AffectedService)
                ? request.AffectedService
                : decision.AffectedService,
            Reasoning = decision.Reasoning.Count == 0
                ? ["AI returned a valid decision without reasoning details."]
                : decision.Reasoning,
            DecisionSource = string.IsNullOrWhiteSpace(decision.DecisionSource)
                ? DecisionSource
                : decision.DecisionSource
        };
    }

    private static AiIncidentDecision ParseDecision(string responseText)
    {
        string json = ExtractJsonObject(responseText);
        AiIncidentDecision? decision;

        try
        {
            decision = JsonSerializer.Deserialize<AiIncidentDecision>(json, JsonOptions);
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException("AI response was not valid JSON for the required decision contract.", ex);
        }

        if (decision is null ||
            string.IsNullOrWhiteSpace(decision.IncidentIntent) ||
            string.IsNullOrWhiteSpace(decision.AffectedService))
        {
            throw new InvalidOperationException("AI decision JSON is missing required fields.");
        }

        return decision;
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
