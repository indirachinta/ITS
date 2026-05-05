using ResolvePilot.Domain;

namespace ResolvePilot.Application;

public interface IIncidentResolutionEngine
{
    ResolutionResponse Resolve(IncidentRequest request);
}
