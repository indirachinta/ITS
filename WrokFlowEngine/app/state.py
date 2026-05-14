from typing import TypedDict

from app.models import (
    EmailNotification,
    JiraTicket,
    ResolvePilotHandoff,
    WorkflowOutput,
    WorkflowTraceItem,
)


class WorkflowState(TypedDict, total=False):
    """State passed between LangGraph nodes."""

    handoff: ResolvePilotHandoff
    analysisSummary: str
    actionItems: list[str]
    jiraTicket: JiraTicket
    emailNotification: EmailNotification
    workflowTrace: list[WorkflowTraceItem]
    confidence: float
    workflowStatus: str
    errors: list[str]
    output: WorkflowOutput
