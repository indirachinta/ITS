# ToolCortex - Intelligent MCP Tool Selector

## Problem Statement

Incident response teams often have many tools available, but no consistent, explainable way to choose the right ones quickly. Selection becomes ad hoc, biased toward familiar tools, and difficult to justify after the fact.

## What This Project Solves

ToolCortex provides a deterministic tool-selection layer that evaluates a mixed tool ecosystem and returns the best candidates for a given incident context.

For each recommendation, it returns:
- source type (`custom` or `external`)
- score and confidence
- an explicit ranking reason

## Solution Structure

The solution has been flattened so all projects now live at the repository root:

- `ToolCortex.Domain` - contracts, models, and interfaces
- `ToolCortex.Application` - deterministic scoring and ranking engine
- `ToolCortex.Infrastructure` - in-memory tool registry with sample tool metadata
- `ToolCortex.McpAdapter` - thin MCP-facing adapter over the selection engine
- `ToolCortex.ApiService` - minimal HTTP API used to run the demo flow
- `specs` - source-of-truth implementation specs
- `samples` - example requests and expected outputs

The solution file is [`ToolCortex.slnx`](./ToolCortex.slnx).

## How It Works

1. An `IncidentRequest` enters through the MCP adapter or HTTP API.
2. The adapter delegates directly to the deterministic selection engine.
3. The engine retrieves all candidate tools from the registry.
4. It derives investigation signals from the incident context.
5. Deterministic scoring and ranking rules are applied.
6. The top recommendations are returned in a `SelectorResponse`.
7. Rejected tools can also be included for explainability.

## Project Contracts

Input contract:

```json
{
  "title": "Checkout API failing intermittently",
  "summary": "Customers report random failures during checkout. Errors increased after a recent deployment.",
  "affectedService": "checkout-service",
  "incidentType": "runtime failure",
  "symptomType": "intermittent API failure",
  "severity": "high"
}
```

Response shape:

```json
{
  "incidentSummary": "Checkout API failing intermittently | runtime failure | intermittent API failure | high",
  "recommendedTools": [
    {
      "toolName": "get_service_logs",
      "sourceType": "custom",
      "score": 1.0,
      "confidence": 1.0,
      "reason": "Strong intent alignment with logs investigation needs"
    }
  ],
  "rejectedTools": []
}
```

## Requirements

- .NET SDK 10.0 or later

Why `10.0`:
- `ToolCortex.ApiService` targets `net10.0`
- the other projects target `net8.0`, which can still build under the .NET 10 SDK

## Build The Solution

```bash
dotnet restore .\ToolCortex.slnx
dotnet build .\ToolCortex.slnx
```

## Run The API Service

The demo is now driven through `ToolCortex.ApiService`.

Start the minimal API:

```bash
dotnet run --project .\ToolCortex.ApiService\ToolCortex.ApiService.csproj
```

By default, the development launch settings expose:
- `http://localhost:5041`
- `https://localhost:7146`

Primary endpoint:

```text
POST /mcp/selecttools
```

## Run The Demo Through ApiService

Use the sample requests in [`samples/incident-requests.json`](./samples/incident-requests.json) as the demo input payloads.

What the API-driven demo does:
- creates the in-memory registry
- runs the deterministic engine
- invokes the MCP adapter through the HTTP endpoint
- returns structured JSON selector results for each incident request

## Invoke The API

Example `curl` request:

```bash
curl -X POST "http://localhost:5041/mcp/selecttools" ^
  -H "Content-Type: application/json" ^
  -d "{\"title\":\"Checkout API failing intermittently\",\"summary\":\"Customers report random failures during checkout. Errors increased after a recent deployment.\",\"affectedService\":\"checkout-service\",\"incidentType\":\"runtime failure\",\"symptomType\":\"intermittent API failure\",\"severity\":\"high\"}"
```

PowerShell example:

```powershell
$body = @{
    title = "Checkout API failing intermittently"
    summary = "Customers report random failures during checkout. Errors increased after a recent deployment."
    affectedService = "checkout-service"
    incidentType = "runtime failure"
    symptomType = "intermittent API failure"
    severity = "high"
} | ConvertTo-Json

Invoke-RestMethod `
  -Method Post `
  -Uri "http://localhost:5041/mcp/selecttools" `
  -ContentType "application/json" `
  -Body $body
```

## Sample Output

Representative response:

```json
{
  "incidentSummary": "Checkout API failing intermittently | runtime failure | intermittent API failure | high",
  "recommendedTools": [
    {
      "toolName": "get_service_logs",
      "sourceType": "custom",
      "score": 1,
      "confidence": 1,
      "reason": "Strong intent alignment with logs investigation needs | Preferred because it is a direct operational or specialized diagnostic tool | Strong match for runtime failure and intermittent error analysis | Aligned with 4 preferred investigation scenario signal(s)"
    },
    {
      "toolName": "get_recent_deployments",
      "sourceType": "custom",
      "score": 0.952,
      "confidence": 0.9688,
      "reason": "Strong intent alignment with deployments investigation needs | Preferred because it is a direct operational or specialized diagnostic tool | Boosted because the incident explicitly mentions a recent deployment or release change | Aligned with 2 preferred investigation scenario signal(s)"
    },
    {
      "toolName": "get_service_health",
      "sourceType": "custom",
      "score": 0.9149999999999999,
      "confidence": 0.94475,
      "reason": "Strong intent alignment with health investigation needs | Preferred because it is a direct operational or specialized diagnostic tool | Important for validating service stability while runtime failures are active | Aligned with 3 preferred investigation scenario signal(s)"
    }
  ]
}
```

## Supporting Files

- sample requests: `samples/incident-requests.json`
- expected output snapshot: `samples/expected-selector-output.json`
- API scratch file: [`ToolCortex.ApiService/ToolCortex.ApiService.http`](./ToolCortex.ApiService/ToolCortex.ApiService.http)

## Scope

Intentionally not included:

- real external MCP integrations
- network-based dynamic tool discovery
- LLM-based orchestration
- auth, databases, and deployment infrastructure
- production runtime hardening

This repository is a focused showcase of deterministic tool selection behavior.
