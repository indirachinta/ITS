import json
import sys
from pathlib import Path

PROJECT_ROOT = Path(__file__).resolve().parents[1]
if str(PROJECT_ROOT) not in sys.path:
    sys.path.insert(0, str(PROJECT_ROOT))

from app.graph import build_investigation_graph


def main():
    workflow = build_investigation_graph()

    initial_state = {
        "incident_id": "INC-1001",
        "title": "Checkout API failing intermittently",
        "summary": "Customers report random checkout failures after deployment v2.3.1",
        "affected_service": "checkout-service",
        "severity": "high",
        "incident_intent": "deployment_related_failure",
        "recommended_tools": ["get_service_logs"],

        "evidence": [],
        "executed_tools": [],
        "investigation_steps": [],

        "root_cause_hypothesis": "",
        "confidence": 0.0,
        "recommended_actions": [],
        "workflow_status": "not_started",
    }

    result = workflow.invoke(initial_state)

    print("\nDeep Investigation Result:")
    print(json.dumps(result, indent=2))


if __name__ == "__main__":
    main()
