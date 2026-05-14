# Component 3 Workflow Spec: ResolvePilot.WorkflowEngine

## Purpose

`ResolvePilot.WorkflowEngine` is a Python LangGraph workflow engine for post-tool incident workflow orchestration.

It receives a ResolvePilot handoff after incident tools have already run, analyzes the completed tool execution results, creates mocked follow-up artifacts, and returns a structured workflow result.

## Core Rule

LangGraph does not execute tools.

Tool execution is owned by ResolvePilot and IncidentTools.Core. This component only reads the handoff payload and existing `toolExecutionResults`.

## Workflow Steps

The POC workflow performs these steps:

1. Receive ResolvePilot handoff.
2. Validate required handoff fields.
3. Analyze tool execution results.
4. Generate deep analysis summary using GitHub Models.
5. Generate action items.
6. Create mocked Jira ticket.
7. Assign mocked Jira ticket.
8. Send mocked email.
9. Return final structured workflow result.

## Inputs

The workflow receives the ResolvePilot handoff contract defined in [resolvepilot-handoff-contract.md](./resolvepilot-handoff-contract.md).

Required high-level input fields:

- `incidentIntent`
- `affectedService`
- `resolutionPath`
- `toolsExecuted`
- `toolExecutionResults`
- `resolutionSummary`
- `recommendedAction`
- `confidence`
- `reasoning`
- `workflowRequired`
- `investigationFindings`
- `decisionSource`

## LangGraph State

The workflow state should remain small and explicit:

- `handoff`: Original ResolvePilot handoff.
- `analysisSummary`: Deep analysis generated with GitHub Models.
- `actionItems`: Action items derived from the handoff and analysis.
- `jiraTicket`: Mocked Jira ticket result.
- `emailNotification`: Mocked email notification result.
- `workflowTrace`: Ordered trace of workflow steps.
- `confidence`: Final workflow confidence score.
- `workflowStatus`: Final workflow status.

## Nodes

### Receive Handoff

Accepts the ResolvePilot handoff payload and stores it in workflow state.

If `workflowRequired` is false, the workflow may return a completed result with no Jira or email actions attempted.

### Validate Handoff

Validates that the required contract fields are present and usable.

Validation must not attempt to execute or re-execute tools.

### Analyze Tool Execution Results

Reads `toolsExecuted`, `toolExecutionResults`, `reasoning`, `investigationFindings`, `resolutionSummary`, and `recommendedAction`.

The node prepares analysis context for GitHub Models.

Each `toolExecutionResults` item may contribute:

- `toolName`
- `status`
- `summary`
- `evidence`
- `confidence`
- `rawData`

### Generate Deep Analysis Summary

Uses GitHub Models only for deep analysis.

GitHub Models should summarize:

- What happened.
- What upstream tools found.
- How findings relate to the affected service.
- Why the recommended action is appropriate.
- How ResolvePilot reasoning supports the selected resolution path.
- How `investigationFindings.rootCauseHypothesis` is supported or limited by evidence.
- What uncertainty remains.

GitHub Models must not be used to call tools, simulate tool execution, create Jira tickets, assign tickets, or send email.

### Generate Action Items

Creates a concise list of action items from:

- `recommendedAction`
- `resolutionPath`
- `reasoning`
- `investigationFindings`
- Deep analysis summary

Action items should be practical for an incident responder.

### Create Mocked Jira Ticket

Creates a mocked Jira ticket using data from `/mock-data` JSON files and the current workflow state.

This is not a real Jira integration.

Expected mocked Jira output:

- Ticket key.
- Ticket title.
- Ticket description.
- Severity or priority.
- Assignee.
- Mock URL.
- Mock status.

### Assign Mocked Jira Ticket

Assigns the mocked Jira ticket using assignment data read from `/mock-data` JSON files.

The assignment decision should consider:

- `affectedService`
- `incidentIntent`
- `resolutionPath`
- `confidence`

No real Jira APIs are called.

### Send Mocked Email

Creates a mocked email notification using recipient/template data read from `/mock-data` JSON files.

This is not a real email integration.

Expected mocked email output:

- Recipients.
- Subject.
- Body.
- Mock delivery status.
- Mock provider message ID.

### Return Final Structured Workflow Result

Builds the response defined in [workflow-output-contract.md](./workflow-output-contract.md).

## Mock Data Rule

Mock integrations must read data from `/mock-data` JSON files.

Expected POC mock data categories:

- Jira project or ticket defaults.
- Assignment rules.
- Email recipients.
- Email templates.

The exact JSON filenames can be decided during implementation, but mocked Jira and mocked email behavior must not be hardcoded directly into LangGraph nodes.

## Error Handling

POC error handling should be simple:

- Invalid handoff: return `workflowStatus` of `failed`.
- GitHub Models deep analysis failure: return `workflowStatus` of `failed`.
- Mock Jira creation failure: return `workflowStatus` of `failed`.
- Mock Jira assignment failure: return `workflowStatus` of `failed`.
- Mock email failure: return `workflowStatus` of `partial_success` if analysis and Jira creation succeeded.

Each major step should append an entry to `workflowTrace`.

## Acceptance Criteria

- The workflow receives a ResolvePilot handoff.
- The workflow analyzes existing tool execution results.
- The workflow generates a deep analysis summary.
- The workflow generates action items.
- The workflow creates a mocked Jira ticket.
- The workflow assigns the mocked Jira ticket.
- The workflow sends a mocked email.
- The workflow returns a structured result.
- LangGraph never executes incident tools.
- Mocked Jira and email integrations read from `/mock-data` JSON files.
- GitHub Models is used only for deep analysis.
- Jira and email remain mocked, not real integrations.
