# V3 — Real ToolCortex HTTP Integration Spec

## Purpose

V3 replaces the mock ToolCortex client with a real HTTP integration to Component 1.

ToolCortex runs as an independent API service.

ResolvePilot must call ToolCortex to get ranked tool recommendations.

## Service Boundary

ResolvePilot must not duplicate ToolCortex scoring logic.

ToolCortex owns:
- tool ranking
- scoring
- confidence
- recommended tools
- rejected tools
- explanation of why tools were selected or rejected

ResolvePilot owns:
- incident interpretation
- FAST/DEEP path decision
- FAST path execution
- final resolution response

## ToolCortex Endpoint

POST /mcp/select-tools

Base URL must come from configuration.

Example:

ToolCortex base URL:
http://localhost:5001

Full endpoint:
http://localhost:5001/mcp/select-tools

## Configuration

Use appsettings:

{
  "ToolCortex": {
    "BaseUrl": "http://localhost:5001"
  }
}

## Request Contract

ResolvePilot sends:

{
  "title": "string",
  "summary": "string",
  "affectedService": "string",
  "severity": "string",
  "symptoms": ["string"],
  "incidentType": "string"
}

## Response Contract

ToolCortex returns:

{
  "incidentSummary": "string",
  "recommendedTools": [
    {
      "toolName": "string",
      "sourceType": "Custom | External",
      "score": 0.0,
      "confidence": 0.0,
      "reason": "string"
    }
  ],
  "rejectedTools": [
    {
      "toolName": "string",
      "sourceType": "Custom | External",
      "reason": "string"
    }
  ]
}

## Failure Handling

For V3 POC:
- If ToolCortex API fails, throw a clear exception.
- Do not silently fallback to mock ToolCortex.
- Fail fast is acceptable for POC scope.

## Out of Scope

- retries
- circuit breaker
- Polly
- production resilience
- auth between services