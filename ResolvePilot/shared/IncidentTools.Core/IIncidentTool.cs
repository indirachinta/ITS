namespace IncidentTools.Core;

public interface IIncidentTool
{
    string ToolName { get; }

    Task<ToolExecutionResult> ExecuteAsync(
        ToolExecutionRequest request,
        CancellationToken cancellationToken = default);
}
