using ResolvePilot.Domain;

namespace ResolvePilot.Application;

public sealed class IncidentResolutionEngine : IIncidentResolutionEngine
{
    public ResolutionResponse Resolve(IncidentRequest request)
    {
        return new ResolutionResponse
        {
            IncidentIntent = "deployment_related_failure",
            AffectedService = request.AffectedService,
            ResolutionPath = "fast",
            ToolsExecuted = [],
            ResolutionSummary = "Mock FAST response generated without LLM, ToolCortex, LangGraph, or real tool execution.",
            RecommendedAction = "Review recent deployments for the affected service and prepare a rollback or hotfix if deployment evidence confirms the failure.",
            Confidence = 0.82,
            Reasoning =
            [
                "V1 foundation is constrained to a mock FAST response.",
                "No external tools were executed.",
                "The response follows the required ResolutionResponse output contract."
            ],
            WorkflowRequired = false,
            InvestigationFindings = null
        };
    }
}
