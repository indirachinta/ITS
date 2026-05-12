# Investigation State Spec

## Purpose

Defines the workflow memory used by LangGraph during deep incident investigation.

## State Fields

- incident_id
- title
- summary
- affected_service
- severity
- incident_intent
- evidence
- executed_tools
- investigation_steps
- root_cause_hypothesis
- confidence
- recommended_actions
- workflow_status

## Design Rule

The workflow must update state step by step.
No node should return free-form unstructured output.