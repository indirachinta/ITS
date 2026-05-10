# ResolvePilot

ResolvePilot is Component 2 of an AI-driven Incident Triage System.

It acts as a spec-governed incident resolution orchestrator that combines:

- runtime specification-driven behavior
- LLM-based incident understanding
- deterministic FAST/DEEP orchestration
- ToolCortex deterministic tool recommendations
- lightweight FAST-path execution
- AI-based final resolution synthesis

---

# Overall System

```text
ToolCortex (Component 1)
↓
ResolvePilot (Component 2)
↓
LangGraph Investigation Engine (Component 3 - future scope)
```

---

# Core Philosophy

ResolvePilot intentionally separates:

- AI interpretation
from
- deterministic orchestration

The goal is to keep:

- reasoning flexible
- orchestration explainable
- workflows governable

---

# Responsibilities

| Responsibility | Owner |
|---|---|
| Incident understanding | ResolvePilot + AI |
| Tool ranking | ToolCortex |
| FAST/DEEP routing | ResolvePilot |
| Tool execution | IncidentTools.Core |
| Final resolution synthesis | ResolvePilot + AI |
| Deep investigation workflows | Future Component 3 |

---

# Runtime Spec-Governed Behavior

ResolvePilot treats specs as runtime orchestration inputs.

Specs are not only development documentation.

ResolvePilot loads runtime specs from `/Specs` and uses them during:

- incident understanding
- orchestration
- response synthesis
- execution policies

Examples:

- incident classification policies
- path decision policies
- response contract policies
- execution policies

---

# AI Layer

ResolvePilot uses:

- Microsoft.Extensions.AI
- IChatClient abstraction

This keeps the AI layer model-agnostic.

Current experimentation provider:

- GitHub Models

Future providers may include:

- Azure OpenAI
- OpenAI
- Ollama
- local models

---

# FAST vs DEEP Resolution

## FAST

Used when:

- confidence is high
- issue scope is isolated
- limited tools are sufficient
- deterministic execution is enough

ResolvePilot executes lightweight tools and returns structured evidence.

Examples:

- deployment regressions
- isolated runtime failures
- clear service degradation

---

## DEEP

Used when:

- confidence is low
- dependencies are involved
- evidence is incomplete
- workflow orchestration is required

ResolvePilot does not implement DEEP workflows.

Instead, it returns a structured handoff response for future Component 3 orchestration.

---

# Shared Tool Execution

Tool execution abstractions are shared through:

```text
shared/IncidentTools.Core
```

Reason:

- ResolvePilot FAST execution should reuse tools later in Component 3
- execution behavior remains centralized
- shared contracts simplify orchestration boundaries

---

# Architecture

```text
POST /api/incidents/resolve
        ↓
ResolvePilot API
        ↓
RuntimeSpecLoader
        ↓
RuntimeSpecSet
        ↓
IncidentUnderstandingService
        ↓
Microsoft.Extensions.AI / GitHub Models
        ↓
AI Incident Understanding
        ↓
BuildToolCortexRequest
        ↓
HTTP call to ToolCortex API
        ↓
ToolCortexResponse
        ↓
PathDecisionService
        ↓
FAST or DEEP
   ┌───────────────────────────────┬───────────────────────────────┐
   │ FAST                          │ DEEP                          │
   ↓                               ↓
ExecuteFastPathToolsAsync          BuildDeepHandoffResponse
   ↓                               ↓
ToolExecutionRouter                Structured handoff for Component 3
   ↓                               ↓
Recommended tools                  ResolutionResponse
   ↓
Custom tool?
   ├── Yes
   │     ↓
   │  IncidentTools.Core executes
   │     ↓
   │  ToolExecutionResult
   │
   └── No
         ↓
      External MCP placeholder result
         ↓
      ToolExecutionResult

        ↓
AIResolutionService
        ↓
Microsoft.Extensions.AI / GitHub Models
        ↓
AI Final Resolution
        ↓
BuildFastReasoning
        ↓
ResolutionResponse
```

---

# Runtime Flow

ResolvePilot follows this sequence:

1. Load runtime specs.
2. Use AI to understand the incident.
3. Build a ToolCortex request from the incident and AI understanding.
4. Call ToolCortex over HTTP for ranked tool recommendations.
5. Use deterministic path decision logic to choose FAST or DEEP.
6. If DEEP, return a structured handoff response for future Component 3.
7. If FAST, execute recommended tools.
8. Use AI again to synthesize the final resolution from:
   - original incident request
   - incident understanding
   - ToolCortex response
   - tool execution results
   - runtime specs
9. Return structured resolution response.

---

# Technology

- .NET 8
- ASP.NET Core Minimal API
- Microsoft.Extensions.AI
- GitHub Models
- Spec-Driven Development (SDD)

---

# Current Scope

Implemented:

- runtime spec loading
- real LLM orchestration
- ToolCortex HTTP integration
- FAST/DEEP decision orchestration
- FAST-path tool execution
- AI-based final resolution synthesis
- shared tool execution abstraction

Not implemented:

- LangGraph workflows
- persistent workflow memory
- autonomous investigation loops
- production incident integrations
- root-cause workflow graphs

---

# Example Incident

Input:

```text
Checkout API failing intermittently after recent deployment
```

ResolvePilot:

- interprets incident using runtime specs
- obtains ranked tools from ToolCortex
- decides FAST or DEEP
- executes lightweight tools if FAST
- synthesizes final resolution using AI
- returns structured evidence

---

# Design Direction

ResolvePilot is intentionally designed as:

- a governed orchestration layer
- not a fully autonomous agent

The system prioritizes:

- explainability
- separation of concerns
- deterministic orchestration
- reusable execution boundaries
- model-agnostic AI integration

---

# Future Scope

Component 3 will introduce:

- LangGraph workflows
- multi-step investigation orchestration
- branching investigation paths
- workflow memory/state
- deeper dependency analysis

ResolvePilot intentionally stops before that boundary.