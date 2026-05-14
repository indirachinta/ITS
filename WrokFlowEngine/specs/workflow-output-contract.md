# Workflow Output Contract

## Purpose

This contract defines the structured result returned by `ResolvePilot.WorkflowEngine`.

The output captures deep analysis, action items, mocked Jira activity, mocked email activity, workflow trace, and final confidence.

## Required Output Fields

The POC workflow output must include:

- `workflowStatus`
- `analysisSummary`
- `actionItems`
- `jiraTicket`
- `emailNotification`
- `workflowTrace`
- `confidence`

## Output Shape

```json
{
  "workflowStatus": "succeeded",
  "analysisSummary": "Checkout API latency and 5xx errors increased shortly after deployment deploy-789. The completed tool results suggest a likely deployment-related regression or database timeout behavior affecting checkout requests.",
  "actionItems": [
    "Review checkout-api deployment deploy-789 for changes related to database access.",
    "Inspect database timeout and connection pool metrics for checkout-api.",
    "Prepare rollback or mitigation if error rates remain elevated."
  ],
  "jiraTicket": {
    "key": "MOCK-JIRA-1001",
    "status": "created",
    "title": "Investigate checkout-api latency spike",
    "description": "Mock Jira ticket created from ResolvePilot workflow analysis.",
    "assignee": "sre-oncall",
    "priority": "High",
    "url": "mock://jira/MOCK-JIRA-1001"
  },
  "emailNotification": {
    "status": "sent",
    "recipients": [
      "sre-oncall@example.com",
      "checkout-team@example.com"
    ],
    "subject": "Incident workflow created for checkout-api",
    "body": "Mock email notification for the generated incident workflow.",
    "providerMessageId": "mock-email-1001"
  },
  "workflowTrace": [
    {
      "step": "receive_handoff",
      "status": "completed",
      "message": "ResolvePilot handoff received."
    },
    {
      "step": "deep_analysis",
      "status": "completed",
      "message": "GitHub Models analysis completed."
    },
    {
      "step": "create_jira_ticket",
      "status": "completed",
      "message": "Mock Jira ticket created."
    },
    {
      "step": "send_email",
      "status": "completed",
      "message": "Mock email notification sent."
    }
  ],
  "confidence": 0.86
}
```

## Field Definitions

- `workflowStatus`: Final workflow status.
- `analysisSummary`: Deep analysis summary generated with GitHub Models.
- `actionItems`: Practical next actions generated from the handoff and analysis.
- `jiraTicket`: Mocked Jira ticket result.
- `emailNotification`: Mocked email notification result.
- `workflowTrace`: Ordered list of workflow steps and outcomes.
- `confidence`: Final workflow confidence score from `0.0` to `1.0`.

## Workflow Status Values

Allowed `workflowStatus` values:

- `succeeded`
- `partial_success`
- `failed`
- `skipped`

Use `skipped` when `workflowRequired` is false and no mocked Jira or email action is attempted.

## Jira Ticket Object

`jiraTicket` is a mocked object. No real Jira integration is used.

Required fields:

- `key`
- `status`
- `title`
- `description`
- `assignee`
- `priority`
- `url`

Allowed `jiraTicket.status` values:

- `created`
- `not_created`
- `skipped`

Mocked Jira ticket defaults, assignment options, and project data must be read from `/mock-data` JSON files.

## Email Notification Object

`emailNotification` is a mocked object. No real email provider is used.

Required fields:

- `status`
- `recipients`
- `subject`
- `body`
- `providerMessageId`

Allowed `emailNotification.status` values:

- `sent`
- `failed`
- `not_attempted`
- `skipped`

Mocked email recipients and templates must be read from `/mock-data` JSON files.

## Workflow Trace Object

Each `workflowTrace` item should include:

- `step`: Stable workflow step name.
- `status`: Step outcome.
- `message`: Human-readable step summary.

Optional fields:

- `details`: Structured diagnostic details safe to return to the caller.

## Rules

- LangGraph does not execute tools.
- Mock integrations must read data from `/mock-data` JSON files.
- GitHub Models should be used only for deep analysis.
- Jira and email are mocked, not real integrations.
- The output must not claim real Jira or email side effects occurred.
- The output must not include secrets, credentials, raw prompts, or stack traces.
