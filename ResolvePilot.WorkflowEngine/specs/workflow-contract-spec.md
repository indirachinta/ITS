# Workflow Contract Spec

## Input Contract

The workflow accepts a deep investigation request from ResolvePilot.

Fields:

- incident_id
- title
- summary
- affected_service
- severity
- incident_intent
- recommended_tools

## Output Contract

The workflow returns:

- workflow_status
- root_cause_hypothesis
- confidence
- evidence
- executed_tools
- investigation_steps
- recommended_actions

## Rule

The output must be structured and reusable by ResolvePilot.