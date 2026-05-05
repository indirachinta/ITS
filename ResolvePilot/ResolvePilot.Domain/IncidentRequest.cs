namespace ResolvePilot.Domain;

public sealed record IncidentRequest
{
    public required string Title { get; init; }

    public required string Summary { get; init; }

    public required string AffectedService { get; init; }

    public required string Severity { get; init; }

    public ResolutionMode Mode { get; set; } = ResolutionMode.Auto;
}
