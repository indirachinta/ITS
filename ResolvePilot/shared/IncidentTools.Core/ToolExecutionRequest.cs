namespace IncidentTools.Core;

public sealed record ToolExecutionRequest
{
    public required string ToolName { get; init; }

    public required string AffectedService { get; init; }

    public required string IncidentIntent { get; init; }

    public required string Severity { get; init; }

    public required string IncidentSummary { get; init; }
}
