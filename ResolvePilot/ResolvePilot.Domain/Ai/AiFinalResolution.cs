namespace ResolvePilot.Domain.Ai;

public sealed record AiFinalResolution
{
    public required string ResolutionSummary { get; init; }

    public required string RecommendedAction { get; init; }

    public required double Confidence { get; init; }

    public required IReadOnlyList<string> Reasoning { get; init; }
}
