from app.state import InvestigationState


def initialize_investigation(state: InvestigationState) -> InvestigationState:
    state["workflow_status"] = "in_progress"
    state["investigation_steps"].append(
        {
            "node": "initialize_investigation",
            "status": "completed",
            "detail": "Initialized DEEP investigation workflow after ResolvePilot handoff.",
        }
    )
    return state


def fetch_logs_mock(state: InvestigationState) -> InvestigationState:
    mock_tool_name = (
        state["recommended_tools"][0]
        if state.get("recommended_tools")
        else "get_service_logs"
    )

    state["executed_tools"].append(
        {
            "tool_name": mock_tool_name,
            "execution_mode": "mock",
            "status": "completed",
        }
    )
    state["evidence"].append(
        {
            "source": mock_tool_name,
            "type": "mock_log_summary",
            "detail": (
                "Mock logs show NullReferenceException frequency increased after "
                "deployment v2.3.1."
            ),
        }
    )
    state["investigation_steps"].append(
        {
            "node": "fetch_logs_mock",
            "status": "completed",
            "detail": f"Fetched service logs using mock tool '{mock_tool_name}'.",
        }
    )
    return state


def analyze_evidence(state: InvestigationState) -> InvestigationState:
    state["evidence"].append(
        {
            "source": "workflow_analysis",
            "type": "pattern_observation",
            "detail": "Failure pattern appears correlated with deployment timing.",
        }
    )
    state["investigation_steps"].append(
        {
            "node": "analyze_evidence",
            "status": "completed",
            "detail": "Analyzed accumulated mock evidence.",
        }
    )
    return state


def build_hypothesis(state: InvestigationState) -> InvestigationState:
    state["root_cause_hypothesis"] = (
        "Recent deployment likely introduced a runtime failure in checkout-service."
    )
    state["confidence"] = 0.82
    state["recommended_actions"].append(
        "Validate recent deployment changes and consider rollback if confirmed."
    )
    state["recommended_actions"].append(
        "Inspect checkout-service null handling around the changed release paths."
    )
    state["investigation_steps"].append(
        {
            "node": "build_hypothesis",
            "status": "completed",
            "detail": "Built preliminary root cause hypothesis from mock evidence.",
        }
    )
    return state


def finalize_result(state: InvestigationState) -> InvestigationState:
    state["workflow_status"] = "completed"
    state["investigation_steps"].append(
        {
            "node": "finalize_result",
            "status": "completed",
            "detail": "Finalized structured investigation result for ResolvePilot.",
        }
    )
    return state
