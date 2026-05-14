# ResolvePilot Handoff Contract

## Purpose

This contract defines the payload ResolvePilot sends to `ResolvePilot.WorkflowEngine`.

ResolvePilot sends this handoff after tool execution is complete. `ResolvePilot.WorkflowEngine` treats the payload as read-only workflow input.

## Architecture Rule

LangGraph does not execute tools.

The fields `toolsExecuted` and `toolExecutionResults` describe completed upstream work. They are not instructions for LangGraph to call tools.

## Required Input Fields

The POC handoff must include:

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

## Payload Shape

```json
{
  "incidentIntent": "string",
  "affectedService": "string",
  "resolutionPath": "string",
  "toolsExecuted": [
    "string"
  ],
  "toolExecutionResults": [
    {
      "toolName": "string",
      "status": "string",
      "summary": "string",
      "evidence": [
        "string"
      ],
      "confidence": 0,
      "rawData": {
        "additionalProp1": "string",
        "additionalProp2": "string",
        "additionalProp3": "string"
      }
    }
  ],
  "resolutionSummary": "string",
  "recommendedAction": "string",
  "confidence": 0,
  "reasoning": [
    "string"
  ],
  "workflowRequired": true,
  "investigationFindings": {
    "workflowName": "string",
    "rootCauseHypothesis": "string",
    "evidence": [
      "string"
    ]
  },
  "decisionSource": "string"
}
```

## Field Definitions

- `incidentIntent`: Plain-language intent describing what ResolvePilot is trying to resolve.
- `affectedService`: Service or system affected by the incident.
- `resolutionPath`: The selected follow-up path from ResolvePilot.
- `toolsExecuted`: Names of upstream tools that have already executed.
- `toolExecutionResults`: Structured results from completed upstream tool executions.
- `resolutionSummary`: ResolvePilot summary of the investigation.
- `recommendedAction`: ResolvePilot recommendation for the next action.
- `confidence`: ResolvePilot confidence score from `0.0` to `1.0`.
- `reasoning`: Ordered reasoning statements produced by ResolvePilot.
- `workflowRequired`: Boolean indicating whether Component 3 should perform follow-up workflow actions.
- `investigationFindings`: Structured findings from ResolvePilot, including workflow name, root cause hypothesis, and evidence.
- `decisionSource`: System or component that selected the resolution path.

## Tool Execution Results

Each item in `toolExecutionResults` should include:

- `toolName`
- `status`
- `summary`
- `evidence`
- `confidence`
- `rawData`

Optional fields:

- `executionId`
- `startedAt`
- `completedAt`
- `error`

Allowed `status` values:

- `succeeded`
- `failed`
- `skipped`

Failed or skipped tool results may be included for analysis, but the workflow engine must not retry them.

## Validation Rules

- All required input fields must be present.
- `toolsExecuted` must be an array.
- `toolExecutionResults` must be an array.
- Each tool execution result must include `toolName`, `status`, `summary`, `evidence`, `confidence`, and `rawData`.
- `confidence` must be a number from `0.0` to `1.0`.
- Each tool execution result `confidence` must be a number from `0.0` to `1.0`.
- `reasoning` must be an array.
- `workflowRequired` must be a boolean.
- `investigationFindings` must be an object with `workflowName`, `rootCauseHypothesis`, and `evidence`.
- `investigationFindings.evidence` must be an array.
- Unknown fields may be ignored.

## Ownership Boundary

ResolvePilot owns:

- Tool selection.
- Tool execution.
- Tool retry behavior.
- Tool result collection.
- Resolution path selection.
- Initial recommended action.

`ResolvePilot.WorkflowEngine` owns:

- Deep analysis of the completed handoff data.
- Action item generation.
- Mocked Jira ticket creation.
- Mocked Jira assignment.
- Mocked email notification.
- Final workflow result generation.
