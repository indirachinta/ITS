import json
from pathlib import Path
from typing import Any


class MockFileService:
    """Loads mocked integration data from /mock-data JSON files."""

    def __init__(self, mock_data_dir: Path | None = None) -> None:
        project_root = Path(__file__).resolve().parents[2]
        self.mock_data_dir = mock_data_dir or project_root / "mock-data"

    def read_json(self, file_name: str, default: dict[str, Any] | None = None) -> dict[str, Any]:
        """Read a mock JSON file.

        Empty files are treated as the provided default so the POC can start
        even while a mock file is being filled in.
        """

        path = self.mock_data_dir / file_name
        if not path.exists():
            return default or {}

        content = path.read_text(encoding="utf-8").strip()
        if not content:
            return default or {}

        data = json.loads(content)
        if default:
            return {**default, **data}
        return data

    def jira_ticket_template(self) -> dict[str, Any]:
        return self.read_json(
            "jira-ticket-template.json",
            default={
                "projectKey": "MOCK",
                "issueType": "Bug",
                "priority": "Medium",
                "labels": ["resolvepilot"],
                "status": "created",
            },
        )

    def jira_assignment_rules(self) -> dict[str, Any]:
        return self.read_json(
            "jira-assignment-rules.json",
            default={
                "defaultAssignee": "platform-triage",
                "serviceOwners": {},
                "resolutionPathAssignees": {},
            },
        )

    def email_template(self) -> dict[str, Any]:
        return self.read_json(
            "email-template.json",
            default={
                "from": "resolvepilot@example.com",
                "defaultTo": "platform-triage@example.com",
                "subjectPrefix": "[ResolvePilot Incident]",
                "status": "sent",
            },
        )
