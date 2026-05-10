namespace IncidentTools.Core.Tools;

public sealed class ServiceDependenciesTool : IIncidentTool
{
    private const string MockFileName = "servicedependenciestooldata.json";

    public string ToolName => "get_service_dependencies";

    public async Task<ToolExecutionResult> ExecuteAsync(
        ToolExecutionRequest request,
        CancellationToken cancellationToken = default)
    {
        using var document = await MockToolDataReader.ReadAsync(MockFileName, cancellationToken);
        ServiceDependenciesData data = MockToolDataReader.Deserialize<ServiceDependenciesData>(document.RootElement);
        DependencyDependency riskiest = data.Dependencies
            .OrderByDescending(dependency => dependency.ErrorRatePercent)
            .First();

        return new ToolExecutionResult
        {
            ToolName = ToolName,
            Status = "success",
            Summary = $"{request.AffectedService} depends on {data.Dependencies.Count} services; {riskiest.Name} has the highest error rate.",
            Evidence =
            [
                $"{riskiest.Name} dependency error rate is {riskiest.ErrorRatePercent:0.0}% with p95 latency {riskiest.P95LatencyMs} ms.",
                $"Critical path: {string.Join(" -> ", data.CriticalPath)}.",
                $"{data.Dependencies.Count(dependency => dependency.Status != "healthy")} dependency/dependencies are not healthy."
            ],
            Confidence = Math.Clamp(0.66 + riskiest.ErrorRatePercent / 30, 0, 0.93),
            RawData = MockToolDataReader.ToRawData(document.RootElement)
        };
    }

    private sealed record ServiceDependenciesData(
        IReadOnlyList<string> CriticalPath,
        IReadOnlyList<DependencyDependency> Dependencies);

    private sealed record DependencyDependency(
        string Name,
        string Status,
        double ErrorRatePercent,
        int P95LatencyMs,
        IReadOnlyList<string> RecentSignals);
}
