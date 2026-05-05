# Day 1 V1 Foundation Spec — ResolvePilot

## Component
ResolvePilot — Spec-Governed Incident Resolution API

## Goal
Create the initial .NET solution foundation for Component 2.

ResolvePilot must always return a structured incident resolution response.

## Architecture
- ResolvePilot.Api exposes HTTP endpoint
- ResolvePilot.Domain contains request/response models
- ResolvePilot.Application contains orchestration interface and mock implementation
- ResolvePilot.Infrastructure is reserved for future external integrations
- IncidentTools.Core is a shared library for future reusable tool execution

## Component Boundaries
- Component 1 ToolCortex already exists and will be integrated later
- Component 2 must not implement ToolCortex logic
- Component 2 must not implement LangGraph
- Component 3 LangGraph is future scope
- Shared tools must live in IncidentTools.Core, not inside ResolvePilot only

## Day 1 Scope
Create:
- .NET solution
- Api project
- Domain project
- Application project
- Infrastructure project
- shared IncidentTools.Core project
- runtime specs folder
- sample request
- basic resolve endpoint

## Endpoint
POST /api/incidents/resolve

## Input Contract
IncidentRequest:
- title
- summary
- affectedService
- severity
- mode

Mode values:
- auto
- fast
- deep

## Output Contract
ResolutionResponse:
- incidentIntent
- affectedService
- resolutionPath
- toolsExecuted
- resolutionSummary
- recommendedAction
- confidence
- reasoning
- workflowRequired

## Day 1 Behavior
No LLM.
No ToolCortex call.
No real tool execution.
No LangGraph call.

Return a structured mock FAST response.

## Success Criteria
- dotnet build succeeds
- API runs
- endpoint returns structured response
- specs folder exists
- shared tool library exists
- no LangGraph implementation exists