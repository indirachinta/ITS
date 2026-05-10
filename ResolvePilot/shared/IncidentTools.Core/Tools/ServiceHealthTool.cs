namespace IncidentTools.Core.Tools;

public sealed class ServiceHealthTool : IIncidentTool
{
    private const string MockFileName = "servicehealthtooldata.json";

    public string ToolName => "get_service_health";

    public async Task<ToolExecutionResult> ExecuteAsync(
        ToolExecutionRequest request,
        CancellationToken cancellationToken = default)
    {
        using var document = await MockToolDataReader.ReadAsync(MockFileName, cancellationToken);
        ServiceHealthData data = MockToolDataReader.Deserialize<ServiceHealthData>(document.RootElement);
        RegionHealth worstRegion = data.Regions
            .OrderByDescending(region => region.ErrorRatePercent)
            .First();

        return new ToolExecutionResult
        {
            ToolName = ToolName,
            Status = data.OverallStatus.Equals("Healthy", StringComparison.OrdinalIgnoreCase) ? "success" : "degraded",
            Summary = $"{request.AffectedService} health is {data.OverallStatus}; worst region is {worstRegion.Region}.",
            Evidence =
            [
                $"{worstRegion.Region} has {worstRegion.AvailabilityPercent:0.00}% availability and {worstRegion.ErrorRatePercent:0.0}% errors.",
                $"Active alert: {data.ActiveAlerts.FirstOrDefault()?.Title ?? "none"}.",
                $"Synthetic checkout probe p95 latency is {data.SyntheticChecks.CheckoutProbeP95Ms} ms."
            ],
            Confidence = data.OverallStatus.Equals("Healthy", StringComparison.OrdinalIgnoreCase) ? 0.74 : 0.89,
            RawData = MockToolDataReader.ToRawData(document.RootElement)
        };
    }

    private sealed record ServiceHealthData(
        string OverallStatus,
        IReadOnlyList<RegionHealth> Regions,
        IReadOnlyList<HealthAlert> ActiveAlerts,
        SyntheticChecks SyntheticChecks);

    private sealed record RegionHealth(
        string Region,
        double AvailabilityPercent,
        double ErrorRatePercent,
        int P95LatencyMs);

    private sealed record HealthAlert(string Title, string Severity, string Signal);

    private sealed record SyntheticChecks(int CheckoutProbeP95Ms, int FailedProbeCount);
}
