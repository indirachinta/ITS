namespace IncidentTools.Core;

public sealed class ToolExecutionService(IEnumerable<IIncidentTool> tools) : IToolExecutionService
{
    private readonly IReadOnlyDictionary<string, IIncidentTool> toolsByName = tools.ToDictionary(
        tool => tool.ToolName,
        StringComparer.OrdinalIgnoreCase);

    public Task<ToolExecutionResult> ExecuteAsync(
        ToolExecutionRequest request,
        CancellationToken cancellationToken = default)
    {
        if (!toolsByName.TryGetValue(request.ToolName, out IIncidentTool? tool))
        {
            throw new InvalidOperationException(
                $"Internal incident tool '{request.ToolName}' is not registered in IncidentTools.Core.");
        }

        return tool.ExecuteAsync(request, cancellationToken);
    }
}
