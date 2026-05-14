# ResolvePilot.WorkflowEngine

## Overview

ResolvePilot.WorkflowEngine is Component 3 of the AI-Driven Intelligent Incident Triage System (ITS).

It is a Python-based LangGraph workflow orchestration engine responsible for handling **Complete workflow execution** after ResolvePilot finishes tool execution.

WorkflowEngine does **not** execute diagnostic tools directly.

Instead, it consumes structured tool execution results from ResolvePilot and performs:

- Deep analysis using GitHub Models
- Workflow orchestration
- Mock Jira ticket creation
- Mock ticket assignment
- Mock email notification
- Structured workflow summarization

---

# Architecture Position

```text
ToolCortex
    ↓
ResolvePilot
    ↓
WorkflowEngine
```

WorkflowEngine begins only after:

1. ResolvePilot understands the incident
2. ToolCortex recommends tools
3. ResolvePilot executes tools through IncidentTools.Core
4. ResolvePilot decides Complete mode

---

# Core Responsibilities

WorkflowEngine is responsible for:

- Deep analysis of tool execution results
- Generating workflow summary
- Generating remediation action items
- Creating mocked Jira tickets
- Assigning mocked tickets
- Sending mocked email notifications
- Returning structured workflow result

WorkflowEngine is NOT responsible for:

- Tool selection
- Incident understanding
- Diagnostic tool execution
- Incident classification
- Direct interaction with IncidentTools.Core

Those responsibilities belong to ResolvePilot and ToolCortex.

---

# Functional Flow

```text
ResolvePilot Complete Response
        ↓
WorkflowEngine API
        ↓
Initialize Workflow
        ↓
Deep Analysis using GitHub Models
        ↓
Generate Action Items
        ↓
Create Mock Jira Ticket
        ↓
Assign Ticket
        ↓
Send Mock Email
        ↓
Finalize Workflow Result
```

---

# Workflow Nodes

The LangGraph workflow contains the following nodes:

| Node | Responsibility |
|---|---|
| initialize_workflow | Initialize workflow state |
| deep_analyze_results | AI analysis of tool execution results |
| create_jira_ticket | Create mocked Jira ticket |
| assign_jira_ticket | Assign mocked ticket |
| send_email_notification | Send mocked email |
| finalize_workflow | Finalize workflow result |

---

# Tech Stack

| Area | Technology |
|---|---|
| Language | Python 3.10+ |
| Workflow Runtime | LangGraph |
| API Framework | FastAPI |
| Web Server | Uvicorn |
| Validation Models | Pydantic |
| HTTP Client | httpx |
| Environment Management | python-dotenv |
| AI Provider | GitHub Models |
| AI Model | openai/gpt-4.1-mini |

---

# AI Layer

WorkflowEngine uses GitHub Models through OpenAI-compatible APIs.

Current model:

```text
openai/gpt-4.1-mini
```

AI is used only for:

- Deep analysis of ResolvePilot tool execution results
- Workflow analysis summarization
- Action item generation

AI is NOT used for:

- Tool execution
- Workflow routing
- Ticket creation logic
- Email execution logic

---

# Project Structure

```text
ResolvePilot.WorkflowEngine
│
├── app
│   ├── __init__.py
│   ├── main.py
│   ├── models.py
│   ├── state.py
│   ├── graph.py
│   ├── nodes.py
│   │
│   └── services
│       ├── __init__.py
│       ├── github_model_service.py
│       └── mock_file_service.py
│
├── mock-data
│   ├── jira-ticket-template.json
│   ├── jira-assignment-rules.json
│   └── email-template.json
│
├── specs
│   ├── component3-workflow-spec.md
│   ├── resolvepilot-handoff-contract.md
│   ├── workflow-output-contract.md
│   └── workflow-mode-policy.json
│
├── requirements.txt
├── .env
├── .gitignore
└── README.md
```

---

# Workflow State

Workflow state is managed through LangGraph StateGraph.

Core state contains:

```python
incidentIntent
affectedService
toolExecutionResults
analysisSummary
actionItems
jiraTicket
emailNotification
workflowTrace
confidence
```

The workflow accumulates state step-by-step across nodes.

---

# API Endpoints

## Health Endpoint

```http
GET /health
```

Response:

```json
{
  "status": "healthy",
  "component": "ResolvePilot.WorkflowEngine"
}
```

---

## Workflow Endpoint

```http
POST /api/workflows/investigate
```

Purpose:

Receive ResolvePilot Complete-mode handoff and execute workflow orchestration.

---

# ResolvePilot Handoff Contract

WorkflowEngine receives:

```json
{
  "incidentIntent": "deployment_related_failure",
  "affectedService": "checkout-api",
  "resolutionPath": "Complete",
  "toolsExecuted": [],
  "toolExecutionResults": [],
  "resolutionSummary": "",
  "recommendedAction": "",
  "confidence": 0.85
}
```

---

# Workflow Response Contract

WorkflowEngine returns:

```json
{
  "workflowStatus": "succeeded",
  "analysisSummary": "...",
  "actionItems": [],
  "jiraTicket": {},
  "emailNotification": {},
  "workflowTrace": [],
  "confidence": 0.83
}
```

---

# Mock Integrations

WorkflowEngine uses mocked integrations for POC simplicity.

## Mock Jira Integration

Reads configuration from:

```text
mock-data/jira-ticket-template.json
```

Produces:

- mocked ticket ID
- mocked ticket assignment
- mocked Jira URL

---

## Mock Email Integration

Reads configuration from:

```text
mock-data/email-template.json
```

Produces:

- mocked recipients
- mocked subject
- mocked provider message ID

---

# Runtime Configuration

## Environment Variables

`.env`

```env
GITHUB_MODELS_API_KEY=your_token_here
GITHUB_MODELS_ENDPOINT=https://models.github.ai/inference
GITHUB_MODELS_MODEL=openai/gpt-4.1-mini
```

---

# Installation

## Create Virtual Environment

```bash
python -m venv .venv
```

Activate:

### Windows

```bash
.venv\Scripts\activate
```

---

# Install Dependencies

```bash
pip install -r requirements.txt
```

---

# Run API

```bash
uvicorn app.main:app --reload
```

Default URL:

```text
http://127.0.0.1:8000
```

Swagger UI:

```text
http://127.0.0.1:8000/docs
```

---

# Design Principles

## 1. WorkflowEngine Does Not Execute Tools

Tool execution remains centralized in ResolvePilot through IncidentTools.Core.

WorkflowEngine only consumes tool execution results.

---

## 2. Explicit Workflow Orchestration

Workflow steps are explicit LangGraph nodes rather than hidden prompt loops.

---

## 3. Structured AI Outputs

WorkflowEngine returns structured contracts rather than free-form AI responses.

---

## 4. Spec-Driven Development

The implementation follows SDD principles:

```text
Specs
    ↓
AI-generated implementation
    ↓
Validation
    ↓
Refinement
```

---

# Complete Mode Behavior

```text
ResolvePilot
    ↓
Execute tools
    ↓
Trigger WorkflowEngine
    ↓
Deep analysis
    ↓
Workflow orchestration
    ↓
Workflow result returned
```

---

# Fast vs Complete

## Fast Mode

ResolvePilot only:

- executes tools
- generates fast resolution
- returns response immediately

WorkflowEngine is not triggered.

---

## Complete Mode

ResolvePilot:

- executes tools
- triggers WorkflowEngine

WorkflowEngine:

- analyzes tool results
- creates workflow artifacts
- returns workflow result

---

# Current Scope

## Included

- LangGraph workflow orchestration
- GitHub Models analysis
- Mock Jira workflow
- Mock email workflow
- Structured workflow contracts
- FastAPI integration
- Swagger UI

---

## Out of Scope

- Real Jira integration
- Real email provider integration
- Authentication
- Persistent workflow storage
- Distributed orchestration
- Human approval workflows
- Retry policies
- Multi-agent orchestration

---

# Sample Complete Workflow

```text
Checkout API failure after deployment
        ↓
ResolvePilot executes:
    - get_service_logs
    - get_recent_deployments
        ↓
WorkflowEngine analyzes results
        ↓
Detects deployment-related runtime failure
        ↓
Creates Jira ticket
        ↓
Assigns ticket
        ↓
Sends email
        ↓
Returns workflow summary
```

---

# Positioning

ResolvePilot.WorkflowEngine demonstrates how LangGraph can be used for explicit workflow orchestration in enterprise AI systems.

It is intentionally separated from:

- tool selection
- tool execution
- incident classification

to preserve clean architectural boundaries.

```text
ToolCortex selects tools.
ResolvePilot resolves incidents.
WorkflowEngine completes workflows.
```
