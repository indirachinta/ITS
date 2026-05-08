using ResolvePilot.Domain.ToolCortex;

namespace ResolvePilot.Application.ToolCortex;

public interface IToolCortexClient
{
    Task<ToolCortexResponse> SelectToolsAsync(ToolCortexRequest request, CancellationToken cancellationToken = default);
}
