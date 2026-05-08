using ResolvePilot.Domain;
using ResolvePilot.Application.Ai;
using ResolvePilot.Application.Specs;
using ResolvePilot.Application.ToolCortex;
using ResolvePilot.Domain.ToolCortex;

namespace ResolvePilot.Application;

public sealed class IncidentResolutionEngine(
    IRuntimeSpecLoader runtimeSpecLoader,
    IAiDecisionService aiDecisionService,
    IToolCortexClient toolCortexClient,
    PathDecisionService pathDecisionService) : IIncidentResolutionEngine
{
    public async Task<ResolutionResponse> ResolveAsync(IncidentRequest request, CancellationToken cancellationToken = default)
    {
        RuntimeSpecSet runtimeSpecs = await runtimeSpecLoader.LoadAsync(cancellationToken);
        var aiDecision = await aiDecisionService.DecideAsync(request, runtimeSpecs, cancellationToken);
        ToolCortexResponse toolCortexResponse = await toolCortexClient.SelectToolsAsync(
            BuildToolCortexRequest(request, aiDecision.IncidentIntent),
            cancellationToken);
        string resolutionPath = pathDecisionService.Decide(request, aiDecision, toolCortexResponse);
        bool workflowRequired = resolutionPath.Equals("DEEP", StringComparison.OrdinalIgnoreCase);

        return new ResolutionResponse
        {
            IncidentIntent = aiDecision.IncidentIntent,
            AffectedService = aiDecision.AffectedService,
            ResolutionPath = resolutionPath,
            RecommendedTools = toolCortexResponse.RecommendedTools,
            ToolsExecuted = [],
            ResolutionSummary = workflowRequired
                ? "This incident requires deeper multi-step investigation before confident resolution."
                : aiDecision.ResolutionSummary,
            RecommendedAction = workflowRequired
                ? "Route to Component 3 LangGraph investigation in future scope."
                : aiDecision.RecommendedAction,
            Confidence = aiDecision.Confidence,
            Reasoning = BuildReasoning(aiDecision.Reasoning, toolCortexResponse, resolutionPath),
            WorkflowRequired = workflowRequired,
            InvestigationFindings = null,
            DecisionSource = aiDecision.DecisionSource
        };
    }

    private static ToolCortexRequest BuildToolCortexRequest(IncidentRequest request, string incidentType)
    {
        return new ToolCortexRequest
        {
            Title = request.Title,
            Summary = request.Summary,
            AffectedService = request.AffectedService,
            Severity = request.Severity,
            Symptoms = ExtractSymptoms(request),
            IncidentType = incidentType
        };
    }

    private static IReadOnlyList<string> ExtractSymptoms(IncidentRequest request)
    {
        return $"{request.Title}. {request.Summary}"
            .Split(['.', ';'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    }

    private static IReadOnlyList<string> BuildReasoning(
        IReadOnlyList<string> aiReasoning,
        ToolCortexResponse toolCortexResponse,
        string resolutionPath)
    {
        List<string> reasoning = [.. aiReasoning];

        if (toolCortexResponse.RecommendedTools.Count > 0)
        {
            reasoning.Add($"ToolCortex returned {toolCortexResponse.RecommendedTools.Count} ranked recommendation(s).");
        }

        reasoning.Add($"ResolvePilot selected the {resolutionPath} path.");

        return reasoning;
    }
}
