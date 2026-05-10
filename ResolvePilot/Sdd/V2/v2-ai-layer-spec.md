# V2 — AI Layer Spec

## Purpose

ResolvePilot must use a real LLM call in V2.

The AI layer must be model-agnostic.

## Technology

Use:
- Microsoft.Extensions.AI
- IChatClient abstraction

Initial provider:
- GitHub Models

## Design Rules

- Business logic must not depend directly on GitHub Models.
- ResolvePilot should depend on IChatClient.
- Provider configuration should come from configuration.
- API keys must not be committed.
- Use user secrets or environment variables.
- The LLM must return structured JSON only.
- Invalid LLM JSON must be handled safely.

## AI Decision Output

The model should return:

{
  "incidentIntent": "deployment_related_failure | single_service_runtime_failure | dependency_failure | data_issue | unknown",
  "affectedService": "string",
  "confidence": 0.0,
  "requiresDeepInvestigation": true,
  "reasoning": ["string"]
}

## Decision Source

Responses using real AI should set:

decisionSource = "runtime-spec-governed-ai-github-models"