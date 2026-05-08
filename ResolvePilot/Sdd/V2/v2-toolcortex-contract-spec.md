# V2 — ToolCortex Contract Spec

## Purpose

ResolvePilot depends on ToolCortex, Component 1, for deterministic ranked tool recommendations.

ToolCortex answers:
Which tools should be used, in what order, and why?

ResolvePilot answers:
Should this incident follow FAST or DEEP resolution path?

## Endpoint

POST /mcp/select-tools

## Request Contract

Fields:
- title
- summary
- affectedService
- severity
- symptoms
- incidentType

## Response Contract

Fields:

incidentSummary
string
recommendedTools (array)
toolName
sourceType
score
confidence
reason
rejectedTools (array, optional)
toolName
sourceType
reason

## Design Rules

- ResolvePilot must not duplicate ToolCortex scoring.
- ResolvePilot may use ToolCortex recommendations to decide FAST or DEEP.
- V2 may use a mock ToolCortex client.
- Request and response samples must be maintained under /Samples.
- Real HTTP integration can come later if needed.