using IncidentTools.Core;
using ResolvePilot.Domain.ToolCortex;

namespace ResolvePilot.Application;

public sealed class ExternalMcpToolExecutionPlaceholder
{
    public bool CanHandle(RecommendedTool tool)
    {
        return tool.SourceType == ToolSourceType.External;
    }

    public ToolExecutionResult BuildResult(RecommendedTool tool)
    {
        return new ToolExecutionResult
        {
            ToolName = tool.ToolName,
            Status = "planned_external_mcp_execution",
            Summary = "External MCP tool execution is recognized but not implemented in V3.",
            Evidence =
            [
                "Tool sourceType is External.",
                "External MCP execution will be handled by a future adapter."
            ],
            Confidence = 0,
            RawData = new Dictionary<string, object?>
            {
                ["futureAdapter"] = "ExternalMcpToolAdapter",
                ["scope"] = "future"
            }
        };
    }
}
