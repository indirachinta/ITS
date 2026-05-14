# ITS — Intelligent Incident Triage System

## Overview

ITS (Intelligent Incident Triage System) is a spec-driven AI-powered incident triage and workflow orchestration platform designed to demonstrate enterprise-style AI system architecture using explicit orchestration, structured contracts, reusable tool execution boundaries, and workflow automation.

The system intentionally separates responsibilities across specialized components instead of hiding behavior inside prompts or monolithic AI agents.

ITS demonstrates:

- Spec-driven AI engineering
- Deterministic orchestration
- Explainable tool selection
- Structured AI outputs
- Shared execution boundaries
- Cross-ecosystem workflow orchestration
- Explicit workflow state management

---

# High-Level Architecture

```text
                    ┌──────────────────┐
                    │   User / API     │
                    └────────┬─────────┘
                             │
                             ▼
                ┌────────────────────────┐
                │     ResolvePilot       │
                │ Incident Resolution    │
                │     Orchestrator       │
                └────────┬───────────────┘
                         │
         ┌───────────────┴────────────────┐
         │                                │
         ▼                                ▼
┌───────────────────┐         ┌─────────────────────┐
│    ToolCortex     │         │ IncidentTools.Core │
│ Intelligent Tool  │         │ Shared Tool Layer  │
│     Selection     │         │  Tool Execution    │
└───────────────────┘         └─────────────────────┘
                                               │
                                               ▼
                                ┌────────────────────────┐
                                │    WorkflowEngine      │
                                │ LangGraph Workflow     │
                                │    Orchestration       │
                                └────────────────────────┘
```

---

# System Components

| Component | Responsibility |
|---|---|
| ToolCortex | Intelligent tool selection |
| ResolvePilot | Incident resolution orchestration |
| IncidentTools.Core | Shared tool execution |
| WorkflowEngine | Complete workflow orchestration |

---

# Functional Flow

## 1. Incident Request

User sends incident request to ResolvePilot.

Example:

```json
{
  "title": "Checkout API error rate increased after deployment",
  "summary": "The checkout API began returning elevated 500 responses shortly after the latest production deployment.",
  "affectedService": "checkout-api",
  "severity": "sev2",
  "mode": "Complete"
}
```

---

## 2. Incident Understanding

ResolvePilot uses GitHub Models to:

- classify incident intent
- identify affected service
- estimate confidence
- generate reasoning

Example:

```text
deployment_related_failure
```

---

## 3. Tool Selection

ResolvePilot calls ToolCortex.

ToolCortex:

- evaluates incident context
- ranks diagnostic tools
- explains tool recommendations

Example tools:

```text
get_service_logs
get_recent_deployments
get_service_health
```

---

## 4. Tool Execution

ResolvePilot executes tools through IncidentTools.Core.

IncidentTools.Core:

- centralizes execution behavior
- reads mock operational data
- returns structured tool execution results

Example result:

```json
{
  "toolName": "get_service_logs",
  "status": "success",
  "summary": "checkout-api logs show 4.2% errors over 15 minutes",
  "confidence": 0.83
}
```

---

## 5. Resolution Mode

ITS supports three modes:

| Mode | Behavior |
|---|---|
| Fast | ResolvePilot-only resolution |
| Complete | ResolvePilot + WorkflowEngine |
| Auto | Policy-based Fast or Complete |

---

# Fast Mode Flow

```text
ResolvePilot
    ↓
ToolCortex selects tools
    ↓
IncidentTools.Core executes tools
    ↓
ResolvePilot generates resolution
    ↓
Return fast response
```

Fast mode does NOT trigger WorkflowEngine.

---

# Complete Mode Flow

```text
ResolvePilot
    ↓
ToolCortex selects tools
    ↓
IncidentTools.Core executes tools
    ↓
ResolvePilot sends toolExecutionResults
    ↓
WorkflowEngine
    ↓
Deep analysis
    ↓
Mock Jira ticket
    ↓
Mock assignment
    ↓
Mock email
    ↓
Workflow result returned
```

---

# Auto Mode Flow

```text
ResolvePilot
    ↓
Policy evaluation
    ↓
Fast or Complete selected
```

Auto mode uses policy rules to choose the appropriate path.

---

# Core Design Principles

## 1. Tool Selection Is Externalized

Tool selection is owned by ToolCortex rather than embedded inside prompts.

---

## 2. Tool Execution Is Centralized

IncidentTools.Core provides reusable execution behavior shared across components.

---

## 3. Workflow Orchestration Is Explicit

WorkflowEngine uses LangGraph StateGraph rather than hidden agent loops.

---

## 4. AI Outputs Are Structured

All responses use explicit contracts.

---

## 5. Components Have Clear Boundaries

| Concern | Owner |
|---|---|
| Tool selection | ToolCortex |
| Tool execution | IncidentTools.Core |
| Incident resolution | ResolvePilot |
| Workflow orchestration | WorkflowEngine |

---

# Component Details

---

# 1. ToolCortex

## Purpose

Spec-driven intelligent tool selection engine.

## Responsibilities

- Tool ranking
- Tool recommendation
- Explainable scoring
- Context-aware selection

## Tech Stack

| Area | Technology |
|---|---|
| Runtime | .NET 10 |
| Framework | ASP.NET Core |
| Architecture | MCP-style tool boundary |
| Data | JSON/YAML specs |

## Example Endpoint

```http
POST /mcp/selecttools
```

---

# 2. ResolvePilot

## Purpose

Main incident resolution orchestrator.

## Responsibilities

- Incident understanding
- AI reasoning
- ToolCortex integration
- Tool execution orchestration
- Fast response generation
- WorkflowEngine triggering
- Public response shaping

## Tech Stack

| Area | Technology |
|---|---|
| Runtime | .NET 10 |
| Framework | ASP.NET Core Web API |
| AI Layer | Microsoft.Extensions.AI |
| AI Provider | GitHub Models |
| API Docs | Swagger |

## Example Endpoint

```http
POST /api/incidents/resolve
```

---

# 3. IncidentTools.Core

## Purpose

Shared tool execution layer.

## Responsibilities

- Execute diagnostic tools
- Read mock operational data
- Return structured results

## Example Tools

```text
get_service_logs
get_recent_deployments
get_service_health
```

## Tech Stack

| Area | Technology |
|---|---|
| Runtime | .NET 10 |
| Type | Shared class library |
| Data | JSON mock streams |

---

# 4. WorkflowEngine

## Purpose

Complete workflow orchestration engine.

## Responsibilities

- Deep analysis of tool results
- Action item generation
- Mock Jira ticket creation
- Mock assignment
- Mock email notification
- Workflow summarization

## Important Rule

WorkflowEngine does NOT execute diagnostic tools.

It only consumes ResolvePilot tool execution results.

## Tech Stack

| Area | Technology |
|---|---|
| Runtime | Python 3.10+ |
| Workflow Runtime | LangGraph |
| API Framework | FastAPI |
| Web Server | Uvicorn |
| Validation | Pydantic |
| AI Provider | GitHub Models |

## Example Endpoint

```http
POST /api/workflows/investigate
```

---

# AI Layer

ITS uses GitHub Models through OpenAI-compatible APIs.

Current model:

```text
openai/gpt-4.1-mini
```

Used in:

| Component | Usage |
|---|---|
| ResolvePilot | Incident understanding |
| ResolvePilot | Fast resolution generation |
| WorkflowEngine | Deep workflow analysis |

---

# GitHub Models Configuration

## .NET

```json
"Ai": {
  "Provider": "GitHubModels",
  "GitHubModels": {
    "Endpoint": "https://models.github.ai/inference",
    "Model": "openai/gpt-4.1-mini"
  }
}
```

---

## Python

```env
GITHUB_MODELS_API_KEY=your_token_here
GITHUB_MODELS_ENDPOINT=https://models.github.ai/inference
GITHUB_MODELS_MODEL=openai/gpt-4.1-mini
```

---

# Public Response Contracts

---

# Fast Response

```json
{
  "incidentIntent": "deployment_related_failure",
  "affectedService": "checkout-api",
  "resolutionPath": "Fast",
  "resolutionSummary": "...",
  "recommendedAction": "...",
  "reasoning": [],
  "confidence": 0.85,
  "workflowTriggered": false,
  "additionalDetails": {
    "toolsExecuted": [],
    "toolExecutionResults": []
  }
}
```

---

# Complete Response

```json
{
  "incidentIntent": "deployment_related_failure",
  "affectedService": "checkout-api",
  "resolutionPath": "Complete",
  "workflowTriggered": true,
  "workflowSummary": "...",
  "workflowResult": {
    "workflowStatus": "succeeded",
    "analysisSummary": "...",
    "actionItems": [],
    "jiraTicket": {},
    "emailNotification": {},
    "workflowTrace": [],
    "confidence": 0.83
  },
  "additionalDetails": {
    "toolsExecuted": [],
    "toolExecutionResults": []
  }
}
```

---

# Repository Structure

```text
ITS-main
│
├── ToolCortex
│
├── ResolvePilot
│
├── IncidentTools.Core
│
├── ResolvePilot.WorkflowEngine
│
└── README.md
```

---

# Local Run Order

---

# 1. Run ToolCortex

```bash
dotnet run
```

---

# 2. Run WorkflowEngine

```bash
python -m venv .venv
.venv\Scripts\activate
pip install -r requirements.txt
uvicorn app.main:app --reload
```

Swagger:

```text
http://127.0.0.1:8000/docs
```

---

# 3. Run ResolvePilot

```bash
dotnet run
```

Swagger opens from launch profile.

---

# Mock Integrations

Current implementation intentionally uses mocked integrations:

| Integration | Status |
|---|---|
| Jira | Mocked |
| Email | Mocked |
| Service logs | Mocked |
| Deployments | Mocked |

All mock data is stored in JSON files.

---

# Current Scope

## Included

- Tool selection
- Tool execution
- Fast resolution
- Complete workflow orchestration
- GitHub Models integration
- LangGraph orchestration
- Mock Jira/email integrations
- Structured contracts
- Swagger/FastAPI docs

---

## Out of Scope

- Real Jira integration
- Real email provider
- Authentication
- Persistent workflow storage
- Distributed orchestration
- Human approval workflows
- Production observability
- Multi-agent orchestration

---

# Development Philosophy

ITS follows Spec Driven Development (SDD).

```text
Specifications
        ↓
AI-assisted implementation
        ↓
Validation
        ↓
Refinement
```

Core principles:

- Specs first
- Explicit orchestration
- Deterministic boundaries
- Structured contracts
- Explainable behavior

---

# Positioning

ITS demonstrates how enterprise AI systems can be designed using explicit orchestration instead of prompt-only agents.

```text
ToolCortex selects tools.
ResolvePilot resolves incidents.
WorkflowEngine completes workflows.
```

The system intentionally separates:

- tool intelligence
- incident resolution
- tool execution
- workflow orchestration

into clean reusable components.
