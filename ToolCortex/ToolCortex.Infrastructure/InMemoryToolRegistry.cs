using ToolCortex.Domain;

namespace ToolCortex.Infrastructure;

public sealed class InMemoryToolRegistry : IToolRegistry
{
    private static readonly IReadOnlyList<ToolDefinition> Tools =
    [
        new(
            "get_incident_details",
            ToolSourceType.Custom,
            "Fetches incident metadata and timeline from internal system.",
            ["incident-details"],
            ["incidentId"],
            ["incident", "timeline", "details"],
            ["file system only"],
            0.60,
            "Incident detail packet with timeline"),
        new(
            "get_service_logs",
            ToolSourceType.Custom,
            "Returns scoped runtime logs for a service.",
            ["logs", "runtime"],
            ["serviceName", "timeRange"],
            ["runtime", "failure", "error", "intermittent"],
            ["contract mismatch"],
            0.75,
            "Filtered service logs"),
        new(
            "get_recent_deployments",
            ToolSourceType.Custom,
            "Lists latest deployments and commit metadata.",
            ["deployments"],
            ["serviceName"],
            ["deployment", "recent deploy", "after release"],
            ["schema-only"],
            0.72,
            "Recent deployment timeline"),
        new(
            "get_service_dependencies",
            ToolSourceType.Custom,
            "Shows service-to-service dependency graph.",
            ["dependencies", "integration"],
            ["serviceName"],
            ["integration", "dependency", "contract mismatch"],
            ["cpu saturation only"],
            0.68,
            "Dependency edges and contract versions"),
        new(
            "get_service_health",
            ToolSourceType.Custom,
            "Checks current health and SLO status.",
            ["health", "runtime"],
            ["serviceName"],
            ["failure", "intermittent", "api failure", "degraded", "latency"],
            ["documentation gap"],
            0.70,
            "Health and SLO summary"),
        new(
            "fetch_docs_search",
            ToolSourceType.External,
            "Searches docs by keyword.",
            ["docs"],
            ["query"],
            ["documentation", "how-to", "guidance"],
            ["runtime failure"],
            0.58,
            "Top docs snippets"),
        new(
            "github_repo_search",
            ToolSourceType.External,
            "Searches repository code and readme snippets.",
            ["repo-search", "docs"],
            ["query"],
            ["repo", "documentation", "integration"],
            ["live incident bridge only"],
            0.65,
            "Repository matches"),
        new(
            "log_query_generic",
            ToolSourceType.External,
            "Generic log search across arbitrary backends.",
            ["logs"],
            ["query"],
            ["error", "runtime"],
            ["schema-only"],
            0.54,
            "Cross-platform log matches"),
        new(
            "api_schema_inspector",
            ToolSourceType.External,
            "Inspects OpenAPI/contract schema artifacts.",
            ["schema", "integration"],
            ["apiSpecPath"],
            ["contract", "schema", "integration"],
            ["infra outage"],
            0.74,
            "Schema diff and validation notes"),
        new(
            "file_content_lookup",
            ToolSourceType.External,
            "Reads local files from metadata index for analysis.",
            ["docs", "repo-search"],
            ["path"],
            ["file", "docs", "reference"],
            ["live runtime diagnostics"],
            0.55,
            "File content excerpts")
    ];

    public IReadOnlyList<ToolDefinition> GetAllTools() => Tools;
}
