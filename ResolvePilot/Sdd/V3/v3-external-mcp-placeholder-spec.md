# V3 — External MCP Tool Execution Placeholder Spec

## Purpose

ToolCortex may recommend external MCP tools.

ResolvePilot V3 should recognize external tools but not execute them yet.

This keeps the hybrid tool ecosystem visible without overbuilding external MCP execution in V3.

## Behavior

When a recommended tool has:

sourceType = External

ResolvePilot should:
- not execute it through IncidentTools.Core
- not fail
- return a placeholder execution result
- clearly mark status as not_supported_in_v3 or planned_external_mcp_execution
- explain that external MCP execution will be handled through a future adapter

## Future Adapter

Future component/extension:

ExternalMcpToolAdapter

Responsibilities:
- connect to external MCP servers
- execute external MCP tools
- normalize results into ToolExecutionResult
- preserve same execution contract

## V3 Rule

External tools can appear in ToolCortex recommendations and rejections internally.

But external tools are not actually executed in V3, and ToolCortex recommendation/rejection lists are not exposed in the public ResolvePilot response.
