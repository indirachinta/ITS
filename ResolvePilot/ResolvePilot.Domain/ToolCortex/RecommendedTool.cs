namespace ResolvePilot.Domain.ToolCortex;

public sealed record RecommendedTool
{
    public required string ToolName { get; init; }

    public required double Score { get; init; }

    public required double Confidence { get; init; }

    public required string Source { get; init; }

    public required IReadOnlyList<string> Reasoning { get; init; }
}
