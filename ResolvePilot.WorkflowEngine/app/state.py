from typing import TypedDict


class InvestigationState(TypedDict):
    incident_id: str
    title: str
    summary: str
    affected_service: str
    severity: str
    incident_intent: str
    recommended_tools: list[str]
    evidence: list[dict[str, str]]
    executed_tools: list[dict[str, str]]
    investigation_steps: list[dict[str, str]]
    root_cause_hypothesis: str
    confidence: float
    recommended_actions: list[str]
    workflow_status: str
