using ResolvePilot.Application.Specs;
using ResolvePilot.Domain;
using ResolvePilot.Domain.Ai;

namespace ResolvePilot.Application.Ai;

public interface IAiIncidentUnderstandingService
{
    Task<AiIncidentUnderstanding> UnderstandAsync(
        IncidentRequest request,
        RuntimeSpecSet runtimeSpecs,
        CancellationToken cancellationToken = default);
}
