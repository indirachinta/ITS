namespace IncidentTools.Core.Tools;

public sealed class RecentDeploymentsTool : IIncidentTool
{
    private const string MockFileName = "recentdeploymentstooldata.json";

    public string ToolName => "get_recent_deployments";

    public async Task<ToolExecutionResult> ExecuteAsync(
        ToolExecutionRequest request,
        CancellationToken cancellationToken = default)
    {
        using var document = await MockToolDataReader.ReadAsync(MockFileName, cancellationToken);
        RecentDeploymentsData data = MockToolDataReader.Deserialize<RecentDeploymentsData>(document.RootElement);
        Deployment candidate = data.Deployments
            .Where(deployment => deployment.Service.Equals(request.AffectedService, StringComparison.OrdinalIgnoreCase) ||
                                 deployment.Service.Equals("checkout-api", StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(deployment => deployment.FinishedAtUtc)
            .First();

        string failedGate = candidate.ValidationGates.FirstOrDefault(gate =>
            gate.Status.Equals("warning", StringComparison.OrdinalIgnoreCase) ||
            gate.Status.Equals("failed", StringComparison.OrdinalIgnoreCase))?.Name ?? "none";

        return new ToolExecutionResult
        {
            ToolName = ToolName,
            Status = "success",
            Summary = $"{candidate.Service} deployment {candidate.Version} finished at {candidate.FinishedAtUtc:u} with {candidate.RiskScore:0.00} risk.",
            Evidence =
            [
                $"Deployment {candidate.Version} by {candidate.InitiatedBy} touched {candidate.ChangedFiles.Count} files.",
                $"Validation gate requiring attention: {failedGate}.",
                $"Rollback target is {candidate.RollbackTarget}."
            ],
            Confidence = Math.Clamp(0.55 + candidate.RiskScore * 0.4, 0, 0.94),
            RawData = MockToolDataReader.ToRawData(document.RootElement)
        };
    }

    private sealed record RecentDeploymentsData(IReadOnlyList<Deployment> Deployments);

    private sealed record Deployment(
        string Service,
        string Version,
        DateTimeOffset FinishedAtUtc,
        string InitiatedBy,
        double RiskScore,
        string RollbackTarget,
        IReadOnlyList<string> ChangedFiles,
        IReadOnlyList<ValidationGate> ValidationGates);

    private sealed record ValidationGate(string Name, string Status, string Details);
}
