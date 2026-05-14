from fastapi import FastAPI

from app.graph import workflow_graph
from app.models import ResolvePilotHandoff, WorkflowOutput


app = FastAPI(
    title="ResolvePilot.WorkflowEngine",
    description="Component 3 LangGraph workflow engine for ResolvePilot incident triage.",
    version="0.1.0",
)


@app.get("/health")
async def health() -> dict[str, str]:
    """Simple health check for local and container probes."""

    return {"status": "healthy", "component": "ResolvePilot.WorkflowEngine"}


@app.post("/api/workflows/investigate", response_model=WorkflowOutput)
async def investigate(handoff: ResolvePilotHandoff) -> WorkflowOutput:
    """Run the POC incident workflow from a ResolvePilot handoff."""

    final_state = await workflow_graph.ainvoke({"handoff": handoff})
    return final_state["output"]
