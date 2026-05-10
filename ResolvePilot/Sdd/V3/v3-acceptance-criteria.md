# V3 Acceptance Criteria

## ToolCortex Integration

- ResolvePilot calls real ToolCortex API over HTTP.
- ToolCortex base URL comes from configuration.
- ToolCortex request contract is maintained.
- ToolCortex response contract is maintained.
- ResolvePilot does not duplicate ToolCortex scoring.
- Mock ToolCortex runtime client is not used in V3 runtime path.

## Internal Tool Execution

- IncidentTools.Core contains internal tool contracts.
- IncidentTools.Core contains mock internal tool implementations.
- ResolvePilot executes tools only for FAST path.
- ResolvePilot does not execute tools for DEEP path.
- ResolvePilot executes maximum 2 recommended tools.
- Only recommended tools are considered for execution.
- Rejected tools are never executed.

## External MCP Placeholder

- External tools are recognized by sourceType = External.
- External tools are not executed through IncidentTools.Core.
- External tools return a placeholder result when selected in FAST path.
- Placeholder result clearly says external MCP execution is future scope.
- System does not fail just because a recommended tool is External.

## Response

- ToolsExecuted contains executed Custom tool names for FAST.
- ToolExecutionResults contains evidence for Custom tools.
- ToolExecutionResults contains placeholder results for selected External tools.
- ToolsExecuted is empty for DEEP.
- ToolExecutionResults is empty for DEEP.
- WorkflowRequired is false for FAST.
- WorkflowRequired is true for DEEP.
- ResolutionSummary reflects available tool evidence.
- RecommendedAction is actionable.

## Existing V2 Guarantees

- Microsoft.Extensions.AI remains the AI abstraction.
- GitHub Models remains isolated in Infrastructure/configuration.
- Runtime specs are still used in prompt.
- LangGraph is not implemented.