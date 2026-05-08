namespace ResolvePilot.Domain.ToolCortex;

public sealed record ToolCortexRequest
{
    public required string Title { get; init; }

    public required string Summary { get; init; }

    public required string AffectedService { get; init; }

    public required string Severity { get; init; }

    public required IReadOnlyList<string> Symptoms { get; init; }

    public required string IncidentType { get; init; }
}
