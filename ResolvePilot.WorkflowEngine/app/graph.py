from langgraph.graph import StateGraph, END

from app.state import InvestigationState
from app.nodes import (
    initialize_investigation,
    fetch_logs_mock,
    analyze_evidence,
    build_hypothesis,
    finalize_result,
)


def build_investigation_graph():
    graph = StateGraph(InvestigationState)

    graph.add_node("initialize_investigation", initialize_investigation)
    graph.add_node("fetch_logs_mock", fetch_logs_mock)
    graph.add_node("analyze_evidence", analyze_evidence)
    graph.add_node("build_hypothesis", build_hypothesis)
    graph.add_node("finalize_result", finalize_result)

    graph.set_entry_point("initialize_investigation")

    graph.add_edge("initialize_investigation", "fetch_logs_mock")
    graph.add_edge("fetch_logs_mock", "analyze_evidence")
    graph.add_edge("analyze_evidence", "build_hypothesis")
    graph.add_edge("build_hypothesis", "finalize_result")
    graph.add_edge("finalize_result", END)

    return graph.compile()
