import hashlib

from app.models import EmailNotification, JiraTicket, WorkflowOutput, WorkflowTraceItem
from app.services.github_model_service import GitHubModelService
from app.services.mock_file_service import MockFileService
from app.state import WorkflowState


mock_files = MockFileService()
github_models = GitHubModelService()


def _trace(step: str, status: str, message: str, details: dict | None = None) -> WorkflowTraceItem:
    return WorkflowTraceItem(step=step, status=status, message=message, details=details)


def _append_trace(state: WorkflowState, item: WorkflowTraceItem) -> list[WorkflowTraceItem]:
    return [*state.get("workflowTrace", []), item]


def _copy_model(model, **updates):
    """Copy a Pydantic model in both Pydantic v1 and v2."""

    if hasattr(model, "model_copy"):
        return model.model_copy(update=updates)
    return model.copy(update=updates)


async def initialize_workflow(state: WorkflowState) -> WorkflowState:
    """Start the workflow and honor workflowRequired=false."""

    handoff = state["handoff"]
    trace = _append_trace(
        state,
        _trace("initialize_workflow", "completed", "ResolvePilot handoff received."),
    )

    workflow_status = "succeeded" if handoff.workflowRequired else "skipped"
    return {
        **state,
        "workflowTrace": trace,
        "workflowStatus": workflow_status,
        "confidence": handoff.confidence,
        "actionItems": [],
        "errors": [],
    }


async def deep_analyze_results(state: WorkflowState) -> WorkflowState:
    """Use GitHub Models for deep analysis of completed tool results."""

    if state.get("workflowStatus") == "skipped":
        return {
            **state,
            "analysisSummary": "Workflow was not required by ResolvePilot.",
            "workflowTrace": _append_trace(
                state,
                _trace("deep_analyze_results", "skipped", "Deep analysis skipped because workflowRequired is false."),
            ),
        }

    try:
        analysis = await github_models.analyze(state["handoff"])
        return {
            **state,
            "analysisSummary": analysis.analysisSummary,
            "actionItems": analysis.actionItems,
            "confidence": analysis.confidence,
            "workflowTrace": _append_trace(
                state,
                _trace("deep_analyze_results", "completed", "GitHub Models deep analysis completed."),
            ),
        }
    except Exception as exc:
        return {
            **state,
            "workflowStatus": "failed",
            "errors": [*state.get("errors", []), str(exc)],
            "workflowTrace": _append_trace(
                state,
                _trace("deep_analyze_results", "failed", "GitHub Models deep analysis failed."),
            ),
        }


async def create_jira_ticket(state: WorkflowState) -> WorkflowState:
    """Create a mocked Jira ticket from mock-data and workflow state."""

    if state.get("workflowStatus") in {"failed", "skipped"}:
        return {
            **state,
            "workflowTrace": _append_trace(
                state,
                _trace("create_jira_ticket", "skipped", "Mock Jira ticket creation skipped."),
            ),
        }

    try:
        handoff = state["handoff"]
        template = mock_files.jira_ticket_template()
        project_key = template["projectKey"]
        ticket_seed = f"{handoff.affectedService}:{handoff.resolutionPath}:{handoff.incidentIntent}"
        ticket_number = int(hashlib.sha1(ticket_seed.encode("utf-8")).hexdigest()[:8], 16) % 10000
        ticket_key = f"{project_key}-{ticket_number:04d}"
        ticket = JiraTicket(
            key=ticket_key,
            status=template["status"],
            title=f"Investigate {handoff.affectedService}: {handoff.incidentIntent}",
            description=state.get("analysisSummary", handoff.resolutionSummary),
            priority=template["priority"],
            assignee=None,
            labels=template["labels"],
            url=f"mock://jira/{ticket_key}",
        )
        return {
            **state,
            "jiraTicket": ticket,
            "workflowTrace": _append_trace(
                state,
                _trace("create_jira_ticket", "completed", "Mock Jira ticket created."),
            ),
        }
    except Exception as exc:
        return {
            **state,
            "workflowStatus": "failed",
            "errors": [*state.get("errors", []), str(exc)],
            "workflowTrace": _append_trace(
                state,
                _trace("create_jira_ticket", "failed", "Mock Jira ticket creation failed."),
            ),
        }


async def assign_jira_ticket(state: WorkflowState) -> WorkflowState:
    """Assign the mocked Jira ticket using mock-data rules."""

    if state.get("workflowStatus") in {"failed", "skipped"} or "jiraTicket" not in state:
        return {
            **state,
            "workflowTrace": _append_trace(
                state,
                _trace("assign_jira_ticket", "skipped", "Mock Jira assignment skipped."),
            ),
        }

    try:
        handoff = state["handoff"]
        rules = mock_files.jira_assignment_rules()
        service_owners = rules.get("serviceOwners", {})
        path_assignees = rules.get("resolutionPathAssignees", {})
        assignee = (
            service_owners.get(handoff.affectedService)
            or path_assignees.get(handoff.resolutionPath)
            or rules["defaultAssignee"]
        )

        ticket = _copy_model(state["jiraTicket"], assignee=assignee)
        return {
            **state,
            "jiraTicket": ticket,
            "workflowTrace": _append_trace(
                state,
                _trace("assign_jira_ticket", "completed", f"Mock Jira ticket assigned to {assignee}."),
            ),
        }
    except Exception as exc:
        return {
            **state,
            "workflowStatus": "failed",
            "errors": [*state.get("errors", []), str(exc)],
            "workflowTrace": _append_trace(
                state,
                _trace("assign_jira_ticket", "failed", "Mock Jira assignment failed."),
            ),
        }


async def send_email_notification(state: WorkflowState) -> WorkflowState:
    """Create a mocked email notification using /mock-data templates."""

    if state.get("workflowStatus") in {"failed", "skipped"} or "jiraTicket" not in state:
        return {
            **state,
            "workflowTrace": _append_trace(
                state,
                _trace("send_email_notification", "skipped", "Mock email notification skipped."),
            ),
        }

    try:
        handoff = state["handoff"]
        template = mock_files.email_template()
        recipients = template.get("recipients") or [template["defaultTo"]]
        subject = f"{template['subjectPrefix']} {handoff.affectedService}"
        body = (
            f"Analysis Summary:\n{state.get('analysisSummary', '')}\n\n"
            f"Recommended Action:\n{handoff.recommendedAction}\n\n"
            f"Jira Ticket:\n{state['jiraTicket'].url}"
        )
        email = EmailNotification(
            status=template["status"],
            recipients=recipients,
            subject=subject,
            body=body,
            providerMessageId=f"mock-email-{state['jiraTicket'].key}",
        )
        return {
            **state,
            "emailNotification": email,
            "workflowTrace": _append_trace(
                state,
                _trace("send_email_notification", "completed", "Mock email notification sent."),
            ),
        }
    except Exception as exc:
        return {
            **state,
            "workflowStatus": "partial_success",
            "errors": [*state.get("errors", []), str(exc)],
            "workflowTrace": _append_trace(
                state,
                _trace("send_email_notification", "failed", "Mock email notification failed."),
            ),
        }


async def finalize_workflow(state: WorkflowState) -> WorkflowState:
    """Build the final API response."""

    status = state.get("workflowStatus", "succeeded")
    if status != "failed" and status != "skipped" and state.get("emailNotification"):
        status = "succeeded"

    output = WorkflowOutput(
        workflowStatus=status,
        analysisSummary=state.get("analysisSummary", ""),
        actionItems=state.get("actionItems", []),
        jiraTicket=state.get("jiraTicket"),
        emailNotification=state.get("emailNotification"),
        workflowTrace=_append_trace(
            state,
            _trace("finalize_workflow", "completed", "Workflow result finalized."),
        ),
        confidence=state.get("confidence", state["handoff"].confidence),
    )
    return {**state, "workflowStatus": status, "workflowTrace": output.workflowTrace, "output": output}
