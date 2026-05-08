using ResolvePilot.Domain;

namespace ResolvePilot.Application;

public interface IIncidentResolutionEngine
{
    Task<ResolutionResponse> ResolveAsync(IncidentRequest request, CancellationToken cancellationToken = default);
}
