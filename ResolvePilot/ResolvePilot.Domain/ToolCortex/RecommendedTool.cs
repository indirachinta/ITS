namespace ResolvePilot.Domain.ToolCortex;

public sealed record RecommendedTool
{
    public required string ToolName { get; init; }

    public required ToolSourceType SourceType { get; init; }

    public required double Score { get; init; }

    public required double Confidence { get; init; }

    public required string Reason { get; init; }
}
