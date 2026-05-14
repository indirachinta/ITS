from typing import Any, Literal

from pydantic import BaseModel, Field


WorkflowStatus = Literal["succeeded", "partial_success", "failed", "skipped"]
StepStatus = Literal["completed", "failed", "skipped"]


class ToolExecutionResult(BaseModel):
    """One completed tool result received from ResolvePilot."""

    toolName: str
    status: str
    summary: str
    evidence: list[str] = Field(default_factory=list)
    confidence: float = Field(ge=0.0, le=1.0)
    rawData: dict[str, Any] = Field(default_factory=dict)


class InvestigationFindings(BaseModel):
    """Structured findings selected by ResolvePilot before this workflow starts."""

    workflowName: str
    rootCauseHypothesis: str
    evidence: list[str] = Field(default_factory=list)


class ResolvePilotHandoff(BaseModel):
    """Input contract for POST /api/workflows/investigate."""

    incidentIntent: str
    affectedService: str
    resolutionPath: str
    toolsExecuted: list[str] = Field(default_factory=list)
    toolExecutionResults: list[ToolExecutionResult] = Field(default_factory=list)
    resolutionSummary: str
    recommendedAction: str
    confidence: float = Field(ge=0.0, le=1.0)
    reasoning: list[str] = Field(default_factory=list)
    workflowRequired: bool
    investigationFindings: InvestigationFindings | None = None
    decisionSource: str


class JiraTicket(BaseModel):
    """Mocked Jira ticket. This is never sent to a real Jira API."""

    key: str
    status: str
    title: str
    description: str
    assignee: str | None = None
    priority: str
    url: str
    labels: list[str] = Field(default_factory=list)


class EmailNotification(BaseModel):
    """Mocked email notification. This is never sent to a real provider."""

    status: str
    recipients: list[str] = Field(default_factory=list)
    subject: str
    body: str
    providerMessageId: str | None = None


class WorkflowTraceItem(BaseModel):
    """Small, caller-safe record of one workflow step."""

    step: str
    status: StepStatus
    message: str
    details: dict[str, Any] | None = None


class WorkflowOutput(BaseModel):
    """Output contract returned by the workflow engine."""

    workflowStatus: WorkflowStatus
    analysisSummary: str
    actionItems: list[str] = Field(default_factory=list)
    jiraTicket: JiraTicket | None = None
    emailNotification: EmailNotification | None = None
    workflowTrace: list[WorkflowTraceItem] = Field(default_factory=list)
    confidence: float = Field(ge=0.0, le=1.0)


class DeepAnalysisResult(BaseModel):
    """Normalized deep-analysis data from GitHub Models."""

    analysisSummary: str
    actionItems: list[str] = Field(default_factory=list)
    confidence: float = Field(ge=0.0, le=1.0)
