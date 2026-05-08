using ResolvePilot.Domain.ToolCortex;

namespace ResolvePilot.Application.ToolCortex;

public sealed class MockToolCortexClient : IToolCortexClient
{
    public Task<ToolCortexResponse> SelectToolsAsync(ToolCortexRequest request, CancellationToken cancellationToken = default)
    {
        IReadOnlyList<RecommendedTool> tools = request.IncidentType.Equals("dependency_failure", StringComparison.OrdinalIgnoreCase)
            ? BuildDependencyTools()
            : BuildFastTools(request);

        ToolCortexResponse response = new()
        {
            IncidentSummary = $"{request.Title} | {request.IncidentType} | {request.AffectedService} | {request.Severity}",
            RecommendedTools = tools,
            RejectedTools =
            [
                new RejectedTool
                {
                    ToolName = "fetch_docs_search",
                    SourceType = "External",
                    Reason = "Limited direct alignment with the primary investigation intent"
                }
            ]
        };

        return Task.FromResult(response);
    }

    private static IReadOnlyList<RecommendedTool> BuildFastTools(ToolCortexRequest request)
    {
        List<RecommendedTool> tools = [];

        if (request.IncidentType.Equals("deployment_related_failure", StringComparison.OrdinalIgnoreCase) ||
            request.Summary.Contains("deploy", StringComparison.OrdinalIgnoreCase) ||
            request.Title.Contains("deploy", StringComparison.OrdinalIgnoreCase))
        {
            tools.Add(new RecommendedTool
            {
                ToolName = "get_recent_deployments",
                Score = 92,
                Confidence = 0.88,
                Source = "custom-enterprise-tool",
                Reasoning =
                [
                    "Incident started after recent deployment.",
                    "Deployment timeline can confirm causality."
                ]
            });
        }

        tools.Add(new RecommendedTool
        {
            ToolName = "get_service_logs",
            Score = 87,
            Confidence = 0.84,
            Source = "custom-enterprise-tool",
            Reasoning = ["Runtime failures require service log inspection."]
        });

        return tools;
    }

    private static IReadOnlyList<RecommendedTool> BuildDependencyTools()
    {
        return
        [
            new RecommendedTool
            {
                ToolName = "get_service_health",
                Score = 91,
                Confidence = 0.86,
                Source = "external-mcp-tool",
                Reasoning = ["Multiple dependencies may be degraded."]
            },
            new RecommendedTool
            {
                ToolName = "get_service_dependencies",
                Score = 88,
                Confidence = 0.84,
                Source = "custom-enterprise-tool",
                Reasoning = ["Dependency chain must be inspected."]
            }
        ];
    }
}
