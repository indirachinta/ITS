using ResolvePilot.Application.Specs;
using ResolvePilot.Domain;
using ResolvePilot.Domain.Ai;

namespace ResolvePilot.Application.Ai;

public interface IAiDecisionService
{
    Task<AiIncidentDecision> DecideAsync(IncidentRequest request, RuntimeSpecSet runtimeSpecs, CancellationToken cancellationToken = default);
}
