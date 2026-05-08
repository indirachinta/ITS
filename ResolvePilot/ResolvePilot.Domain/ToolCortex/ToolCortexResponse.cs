namespace ResolvePilot.Domain.ToolCortex;

public sealed record ToolCortexResponse
{
    public required string IncidentSummary { get; init; }

    public required IReadOnlyList<RecommendedTool> RecommendedTools { get; init; }

    public IReadOnlyList<RejectedTool> RejectedTools { get; init; } = [];
}
