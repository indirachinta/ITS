using ResolvePilot.Domain;
using ResolvePilot.Domain.Ai;
using ResolvePilot.Domain.ToolCortex;

namespace ResolvePilot.Application;

public sealed class PathDecisionService
{
    public string Decide(IncidentRequest request, AiIncidentUnderstanding incidentUnderstanding, ToolCortexResponse toolCortexResponse)
    {
        if (request.Mode == ResolutionMode.Fast)
        {
            return "FAST";
        }

        if (request.Mode == ResolutionMode.Deep)
        {
            return "DEEP";
        }

        bool deepTrigger =
     incidentUnderstanding.IncidentIntent.Equals("dependency_failure", StringComparison.OrdinalIgnoreCase) ||
     request.Severity.Equals("critical", StringComparison.OrdinalIgnoreCase) ||
     incidentUnderstanding.Confidence < 0.75 ||
     MentionsMultipleDependencies(request) ||
     MeanToolConfidence(toolCortexResponse) < 0.75;

        return deepTrigger ? "DEEP" : "FAST";
    }

    private static double MeanToolConfidence(ToolCortexResponse? toolCortexResponse)
    {
        if (toolCortexResponse?.RecommendedTools is null ||
            toolCortexResponse.RecommendedTools.Count == 0)
        {
            return 0;
        }

        return toolCortexResponse
            .RecommendedTools
            .Average(tool => tool.Confidence);
    }

    private static bool MentionsMultipleDependencies(IncidentRequest request)
    {
        string text = $"{request.Title} {request.Summary}";

        return text.Contains("payment", StringComparison.OrdinalIgnoreCase) &&
               text.Contains("inventory", StringComparison.OrdinalIgnoreCase);
    }
}
