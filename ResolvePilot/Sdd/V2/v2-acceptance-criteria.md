# V2 Acceptance Criteria

## Functional Criteria

- Runtime specs are loaded from /Specs.
- Prompt includes incident request and runtime specs.
- AI layer uses Microsoft.Extensions.AI.
- IChatClient is injected through DI.
- GitHub Models is configured as the first provider.
- API key is not committed.
- LLM returns structured JSON.
- Invalid AI JSON is handled safely.
- ToolCortex request/response contracts are represented.
- ResolvePilot decides FAST or DEEP.
- Recommended tools appear in response.
- ToolsExecuted remains empty in V2.
- WorkflowRequired is true only for DEEP.
- LangGraph is not implemented.
- Real tool execution is not implemented.

## FAST Scenario

Input:
Checkout API failing after recent deployment.

Expected:
- incidentIntent = deployment_related_failure
- recommendedTools include deployments/logs
- resolutionPath = FAST
- workflowRequired = false
- toolsExecuted = []

## DEEP Scenario

Input:
Checkout failures across payment and inventory dependencies.

Expected:
- incidentIntent = dependency_failure
- resolutionPath = DEEP
- workflowRequired = true
- toolsExecuted = []
- recommendedAction mentions future deep investigation

## Architecture Criteria

- ResolvePilot owns FAST/DEEP decision.
- ToolCortex owns tool ranking.
- IncidentTools.Core remains reserved for V3 tool execution.
- LangGraph remains Component 3.