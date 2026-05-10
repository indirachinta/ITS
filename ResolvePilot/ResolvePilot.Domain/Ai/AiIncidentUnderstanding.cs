namespace ResolvePilot.Domain.Ai;

public sealed record AiIncidentUnderstanding
{
    public required string IncidentIntent { get; init; }

    public required string AffectedService { get; init; }

    public required double Confidence { get; init; }

    public required IReadOnlyList<string> Reasoning { get; init; }

    public required string DecisionSource { get; init; }
}
