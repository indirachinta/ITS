# V2 — Spec-Governed Decision Orchestration

## Purpose

V2 upgrades ResolvePilot from a static mock response to a spec-governed incident decision orchestrator.

ResolvePilot should:
- load runtime specs from /Specs
- build a governed AI prompt
- call a real LLM through Microsoft.Extensions.AI
- classify incident intent
- consume ToolCortex-style tool recommendations
- decide FAST or DEEP path
- return a structured resolution response

## Existing V1 Foundation

V1 already includes:
- API layer
- Application layer
- Domain layer
- Infrastructure layer
- shared IncidentTools.Core placeholder
- runtime Specs folder
- Sdd folder
- Samples folder
- basic resolve endpoint
- mock structured response

## Runtime Specs

Runtime specs are part of application behavior.

They are not just documentation.

ResolvePilot should load specs from /Specs and include them in the AI decision prompt.

Runtime specs include:
- incident-classification-spec.json
- path-decision-spec.json
- resolution-policy-spec.json
- output-contract-spec.json

## FAST vs DEEP

FAST path:
Used when the incident has clear signal, high confidence, and can later be resolved with simple tool execution.

DEEP path:
Used when the incident has low confidence, missing evidence, multiple services, or needs future LangGraph investigation.

## Mode Behavior

Request mode:
- Auto
- Fast
- Deep

Rules:
- Fast forces FAST
- Deep forces DEEP
- Auto uses runtime specs, AI decision, and ToolCortex recommendations.

## V2 Scope

In scope:
- real LLM decision call
- runtime spec loading
- prompt construction
- structured AI decision parsing
- ToolCortex contract
- FAST/DEEP decision

Out of scope:
- real tool execution
- LangGraph implementation
- Component 3
- root cause workflow