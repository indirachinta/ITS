namespace ToolCortex.Domain;

public enum ToolSourceType
{
    Custom = 0,
    External = 1
}

public sealed record IncidentRequest(
    string Title,
    string Summary,
    string AffectedService,
    string IncidentType,
    string SymptomType,
    string Severity,
    string? Environment = null);

public sealed record ToolDefinition(
    string ToolName,
    ToolSourceType SourceType,
    string Purpose,
    IReadOnlyList<string> SupportedIntents,
    IReadOnlyList<string> RequiredInputs,
    IReadOnlyList<string> PreferredScenarios,
    IReadOnlyList<string> DisallowedScenarios,
    double PriorityWeight,
    string OutputSummary);

public sealed record ToolRecommendation(
    string ToolName,
    ToolSourceType SourceType,
    double Score,
    double Confidence,
    string Reason);

public sealed record RejectedTool(
    string ToolName,
    ToolSourceType SourceType,
    string Reason);

public sealed record SelectorResponse(
    string IncidentSummary,
    IReadOnlyList<ToolRecommendation> RecommendedTools,
    IReadOnlyList<RejectedTool>? RejectedTools);
