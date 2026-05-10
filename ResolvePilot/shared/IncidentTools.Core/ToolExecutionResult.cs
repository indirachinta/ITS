namespace IncidentTools.Core;

public sealed record ToolExecutionResult
{
    public required string ToolName { get; init; }

    public required string Status { get; init; }

    public required string Summary { get; init; }

    public required IReadOnlyList<string> Evidence { get; init; }

    public required double Confidence { get; init; }

    public IReadOnlyDictionary<string, object?> RawData { get; init; } = new Dictionary<string, object?>();
}
