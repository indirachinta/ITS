namespace ResolvePilot.Domain.ToolCortex;

public sealed record RejectedTool
{
    public required string ToolName { get; init; }

    public required ToolSourceType SourceType { get; init; }

    public required string Reason { get; init; }
}
