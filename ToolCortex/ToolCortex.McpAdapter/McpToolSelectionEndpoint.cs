using ToolCortex.Domain;

namespace ToolCortex.McpAdapter;

/// <summary>
/// MCP-facing boundary for tool selection requests.
/// This adapter intentionally stays thin and delegates all selection intelligence to the engine.
/// </summary>
public sealed class McpToolSelectionEndpoint(IToolSelectionEngine engine) : IMcpToolSelectionFacade
{
    /// <summary>
    /// Entry point for MCP-style incident tool selection.
    /// Accepts an incident request and returns the selector response contract.
    /// </summary>
    public SelectorResponse RecommendTools(IncidentRequest request) => HandleToolSelection(request);

    // Keep protocol exposure separate from ranking logic.
    // The deterministic selection engine owns all scoring/ranking behavior.
    private SelectorResponse HandleToolSelection(IncidentRequest request) => engine.SelectTools(request);
}
