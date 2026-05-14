from langgraph.graph import END, START, StateGraph

from app.nodes import (
    assign_jira_ticket,
    create_jira_ticket,
    deep_analyze_results,
    finalize_workflow,
    initialize_workflow,
    send_email_notification,
)
from app.state import WorkflowState


def build_workflow_graph():
    """Build the LangGraph workflow.

    The graph is orchestration only. No node executes incident tools.
    """

    graph = StateGraph(WorkflowState)

    graph.add_node("initialize_workflow", initialize_workflow)
    graph.add_node("deep_analyze_results", deep_analyze_results)
    graph.add_node("create_jira_ticket", create_jira_ticket)
    graph.add_node("assign_jira_ticket", assign_jira_ticket)
    graph.add_node("send_email_notification", send_email_notification)
    graph.add_node("finalize_workflow", finalize_workflow)

    graph.add_edge(START, "initialize_workflow")
    graph.add_edge("initialize_workflow", "deep_analyze_results")
    graph.add_edge("deep_analyze_results", "create_jira_ticket")
    graph.add_edge("create_jira_ticket", "assign_jira_ticket")
    graph.add_edge("assign_jira_ticket", "send_email_notification")
    graph.add_edge("send_email_notification", "finalize_workflow")
    graph.add_edge("finalize_workflow", END)

    return graph.compile()


workflow_graph = build_workflow_graph()
