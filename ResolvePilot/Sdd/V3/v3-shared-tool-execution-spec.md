# V3 — Shared Internal Tool Execution Spec

## Purpose

V3 introduces shared internal tool execution for FAST path resolution.

Internal tools must be implemented in IncidentTools.Core so they can be reused by:
- ResolvePilot Component 2
- future LangGraph Component 3

## Important Boundary

IncidentTools.Core implements only internal/custom enterprise tools.

It should not implement external MCP tools directly.

## Supported Internal Tools

- get_service_logs
- get_recent_deployments
- get_service_health
- get_service_dependencies

## Tool Execution Input

Fields:
- toolName
- affectedService
- incidentIntent
- severity
- incidentSummary

## Tool Execution Output

Fields:
- toolName
- status
- summary
- evidence
- confidence
- rawData

## Execution Rules

- Execute only recommended tools.
- Execute only sourceType = Custom tools in V3.
- Do not execute rejected tools.
- Execute max 2 tools.
- DEEP path executes no tools.

## Out of Scope

- real App Insights
- real Azure DevOps
- real ServiceNow
- LangGraph workflow execution
- external MCP execution