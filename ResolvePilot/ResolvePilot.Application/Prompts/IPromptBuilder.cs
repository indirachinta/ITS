using ResolvePilot.Application.Specs;
using ResolvePilot.Domain;

namespace ResolvePilot.Application.Prompts;

public interface IPromptBuilder
{
    string Build(IncidentRequest request, RuntimeSpecSet runtimeSpecs);
}
