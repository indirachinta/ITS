namespace ResolvePilot.Domain;

using ResolvePilot.Domain.ToolCortex;

public sealed record ResolutionResponse
{
    public required string IncidentIntent { get; init; }

    public required string AffectedService { get; init; }

    public required string ResolutionPath { get; init; }

    public required IReadOnlyList<RecommendedTool> RecommendedTools { get; init; }

    public required IReadOnlyList<string> ToolsExecuted { get; init; }

    public required string ResolutionSummary { get; init; }

    public required string RecommendedAction { get; init; }

    public required double Confidence { get; init; }

    public required IReadOnlyList<string> Reasoning { get; init; }

    public required bool WorkflowRequired { get; init; }

    public InvestigationFindings? InvestigationFindings { get; set; }

    public required string DecisionSource { get; init; }
}
