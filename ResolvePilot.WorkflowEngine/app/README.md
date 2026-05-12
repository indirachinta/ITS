# ResolvePilot.WorkflowEngine

Component 3 of the AI-Driven Incident Triage System.

## Purpose

Stateful incident investigation engine using LangGraph.

## Architecture Role

ResolvePilot handles:
- FAST vs DEEP routing
- deterministic orchestration

WorkflowEngine handles:
- stateful investigations
- graph workflows
- evidence accumulation
- root cause analysis

## Current Scope

V1:
- LangGraph workflow skeleton
- structured investigation state
- explicit workflow nodes
- mocked tool execution

## Next Scope

V2:
- ResolvePilot handoff API
- MCP tool adapters
- branching workflows
- deeper investigation logic