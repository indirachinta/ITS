Implement Day 1 V1 of ResolvePilot using the spec in:

sdd/v1-foundation-spec.md

Important:
- Treat the SDD spec as the source of truth.
- Create a .NET solution named ResolvePilot.
- Create projects:
  - ResolvePilot.Api
  - ResolvePilot.Domain
  - ResolvePilot.Application
  - ResolvePilot.Infrastructure
  - IncidentTools.Core under shared/
- Add proper project references.
- Add domain models:
  - IncidentRequest
  - ResolutionResponse
- Add application interface:
  - IIncidentResolutionEngine
- Add mock implementation:
  - IncidentResolutionEngine
- Add endpoint:
  - POST /api/incidents/resolve
- Return a structured mock FAST response.
- Do not implement LLM.
- Do not call ToolCortex.
- Do not implement LangGraph.
- Do not implement real tool execution.
- Keep IncidentTools.Core as a shared placeholder for future tools.

After implementation, summarize what files you created and how they map to the SDD spec.