using ResolvePilot.Domain;
using ResolvePilot.Application.Ai;
using ResolvePilot.Application.Specs;
using ResolvePilot.Application.ToolCortex;
using ResolvePilot.Domain.ToolCortex;
using IncidentTools.Core;

namespace ResolvePilot.Application;

public sealed class IncidentResolutionEngine(
    IRuntimeSpecLoader runtimeSpecLoader,
    IAiIncidentUnderstandingService incidentUnderstandingService,
    IAiResolutionService aiResolutionService,
    IToolCortexClient toolCortexClient,
    PathDecisionService pathDecisionService,
    IToolExecutionService toolExecutionService,
    ExternalMcpToolExecutionPlaceholder externalMcpToolExecutionPlaceholder) : IIncidentResolutionEngine
{
    public async Task<ResolutionResponse> ResolveAsync(IncidentRequest request, CancellationToken cancellationToken = default)
    {
        RuntimeSpecSet runtimeSpecs = await runtimeSpecLoader.LoadAsync(cancellationToken);
        var incidentUnderstanding = await incidentUnderstandingService.UnderstandAsync(request, runtimeSpecs, cancellationToken);
        ToolCortexResponse toolCortexResponse = await toolCortexClient.SelectToolsAsync(
            BuildToolCortexRequest(request, incidentUnderstanding.IncidentIntent),
            cancellationToken);
        string resolutionPath = pathDecisionService.Decide(request, incidentUnderstanding, toolCortexResponse);
        bool workflowRequired = resolutionPath.Equals("DEEP", StringComparison.OrdinalIgnoreCase);

        if (workflowRequired)
        {
            return BuildDeepHandoffResponse(incidentUnderstanding, toolCortexResponse, resolutionPath);
        }

        IReadOnlyList<ToolExecutionResult> toolExecutionResults =
            await ExecuteFastPathToolsAsync(request, incidentUnderstanding.IncidentIntent, toolCortexResponse, cancellationToken);
        IReadOnlyList<string> toolsExecuted = toolExecutionResults
            .Where(result => result.Status.Equals("success", StringComparison.OrdinalIgnoreCase))
            .Select(result => result.ToolName)
            .ToArray();
        var aiFinalResolution = await aiResolutionService.ResolveAsync(
            request,
            incidentUnderstanding,
            toolCortexResponse,
            toolExecutionResults,
            runtimeSpecs,
            cancellationToken);

        return new ResolutionResponse
        {
            IncidentIntent = incidentUnderstanding.IncidentIntent,
            AffectedService = incidentUnderstanding.AffectedService,
            ResolutionPath = resolutionPath,
            ToolsExecuted = toolsExecuted,
            ToolExecutionResults = toolExecutionResults,
            ResolutionSummary = aiFinalResolution.ResolutionSummary,
            RecommendedAction = aiFinalResolution.RecommendedAction,
            Confidence = aiFinalResolution.Confidence,
            Reasoning = BuildFastReasoning(incidentUnderstanding.Reasoning, toolCortexResponse, aiFinalResolution.Reasoning),
            WorkflowRequired = false,
            InvestigationFindings = null,
            DecisionSource = incidentUnderstanding.DecisionSource
        };
    }

    private async Task<IReadOnlyList<ToolExecutionResult>> ExecuteFastPathToolsAsync(
        IncidentRequest request,
        string incidentIntent,
        ToolCortexResponse toolCortexResponse,
        CancellationToken cancellationToken)
    {
        List<ToolExecutionResult> results = [];

        foreach (RecommendedTool tool in toolCortexResponse.RecommendedTools.Take(2))
        {
            if (tool.SourceType == ToolSourceType.Custom)
            {
                results.Add(await toolExecutionService.ExecuteAsync(
                    new ToolExecutionRequest
                    {
                        ToolName = tool.ToolName,
                        AffectedService = request.AffectedService,
                        IncidentIntent = incidentIntent,
                        Severity = request.Severity,
                        IncidentSummary = request.Summary
                    },
                    cancellationToken));
                continue;
            }

            if (externalMcpToolExecutionPlaceholder.CanHandle(tool))
            {
                results.Add(externalMcpToolExecutionPlaceholder.BuildResult(tool));
            }
        }

        return results;
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

    private static ResolutionResponse BuildDeepHandoffResponse(
        Domain.Ai.AiIncidentUnderstanding incidentUnderstanding,
        ToolCortexResponse toolCortexResponse,
        string resolutionPath)
    {
        return new ResolutionResponse
        {
            IncidentIntent = incidentUnderstanding.IncidentIntent,
            AffectedService = incidentUnderstanding.AffectedService,
            ResolutionPath = resolutionPath,
            ToolsExecuted = [],
            ToolExecutionResults = [],
            ResolutionSummary = "This incident requires deeper multi-step investigation before confident resolution.",
            RecommendedAction = "Route to Component 3 investigation workflow. No FAST tools were executed.",
            Confidence = incidentUnderstanding.Confidence,
            Reasoning = BuildDeepReasoning(incidentUnderstanding.Reasoning, toolCortexResponse, resolutionPath),
            WorkflowRequired = true,
            InvestigationFindings = null,
            DecisionSource = incidentUnderstanding.DecisionSource
        };
    }

    private static IReadOnlyList<string> BuildFastReasoning(
        IReadOnlyList<string> understandingReasoning,
        ToolCortexResponse toolCortexResponse,
        IReadOnlyList<string> finalResolutionReasoning)
    {
        List<string> reasoning = [.. understandingReasoning];

        reasoning.AddRange(toolCortexResponse.RecommendedTools.Select(tool =>
            $"ToolCortex recommended {tool.ToolName}: {tool.Reason}"));
        reasoning.AddRange(finalResolutionReasoning);

        return reasoning;
    }

    private static IReadOnlyList<string> BuildDeepReasoning(
        IReadOnlyList<string> understandingReasoning,
        ToolCortexResponse toolCortexResponse,
        string resolutionPath)
    {
        List<string> reasoning = [.. understandingReasoning];

        if (toolCortexResponse.RecommendedTools.Count > 0)
        {
            reasoning.Add($"ToolCortex returned {toolCortexResponse.RecommendedTools.Count} ranked recommendation(s).");
        }

        reasoning.Add($"ResolvePilot selected the {resolutionPath} path.");
        reasoning.Add("Component 3 handoff required; FAST tool execution and final evidence resolution AI call were skipped.");

        return reasoning;
    }
}
