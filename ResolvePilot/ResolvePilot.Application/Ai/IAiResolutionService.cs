using IncidentTools.Core;
using ResolvePilot.Application.Specs;
using ResolvePilot.Domain;
using ResolvePilot.Domain.Ai;
using ResolvePilot.Domain.ToolCortex;

namespace ResolvePilot.Application.Ai;

public interface IAiResolutionService
{
    Task<AiFinalResolution> ResolveAsync(
        IncidentRequest request,
        AiIncidentUnderstanding incidentUnderstanding,
        ToolCortexResponse toolCortexResponse,
        IReadOnlyList<ToolExecutionResult> toolExecutionResults,
        RuntimeSpecSet runtimeSpecs,
        CancellationToken cancellationToken = default);
}
