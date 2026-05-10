namespace IncidentTools.Core.Tools;

public sealed class ServiceLogsTool : IIncidentTool
{
    private const string MockFileName = "servicelogstooldata.json";

    public string ToolName => "get_service_logs";

    public async Task<ToolExecutionResult> ExecuteAsync(
        ToolExecutionRequest request,
        CancellationToken cancellationToken = default)
    {
        using var document = await MockToolDataReader.ReadAsync(MockFileName, cancellationToken);
        ServiceLogsData data = MockToolDataReader.Deserialize<ServiceLogsData>(document.RootElement);
        LogWindow latest = data.Windows.OrderByDescending(window => window.WindowStartUtc).First();
        LogSignal topSignal = latest.Signals.OrderByDescending(signal => signal.Count).First();

        return new ToolExecutionResult
        {
            ToolName = ToolName,
            Status = latest.ErrorRatePercent >= data.AlertThresholds.ErrorRatePercent ? "success" : "no_actionable_signal",
            Summary = $"{request.AffectedService} logs show {latest.ErrorRatePercent:0.0}% errors over {latest.DurationMinutes} minutes; top signal is {topSignal.ExceptionType}.",
            Evidence =
            [
                $"{latest.TotalErrors} errors from {latest.TotalRequests} requests in the latest window.",
                $"{topSignal.ExceptionType} occurred {topSignal.Count} times at {topSignal.Endpoint}.",
                $"Correlation id sample: {topSignal.SampleCorrelationIds.FirstOrDefault("n/a")}."
            ],
            Confidence = Math.Clamp(0.62 + latest.ErrorRatePercent / 20, 0, 0.95),
            RawData = MockToolDataReader.ToRawData(document.RootElement)
        };
    }

    private sealed record ServiceLogsData(AlertThresholds AlertThresholds, IReadOnlyList<LogWindow> Windows);

    private sealed record AlertThresholds(double ErrorRatePercent);

    private sealed record LogWindow(
        DateTimeOffset WindowStartUtc,
        int DurationMinutes,
        int TotalRequests,
        int TotalErrors,
        double ErrorRatePercent,
        IReadOnlyList<LogSignal> Signals);

    private sealed record LogSignal(
        string ExceptionType,
        string Endpoint,
        int Count,
        IReadOnlyList<string> SampleCorrelationIds);
}
