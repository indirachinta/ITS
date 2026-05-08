namespace ResolvePilot.Domain.Ai;

public sealed record AiIncidentDecision
{
    public required string IncidentIntent { get; init; }


    public required string ResolutionSummary { get; init; }

    public required string RecommendedAction { get; init; }

    public required double Confidence { get; init; }   
    public required string AffectedService { get; init; }


    public required IReadOnlyList<string> Reasoning { get; init; }

    public required string DecisionSource { get; init; }

    //public required bool requiresDeepInvestigation { get; init; }
}
