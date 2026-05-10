namespace IncidentTools.Core;

public interface IToolExecutionService
{
    Task<ToolExecutionResult> ExecuteAsync(
        ToolExecutionRequest request,
        CancellationToken cancellationToken = default);
}
