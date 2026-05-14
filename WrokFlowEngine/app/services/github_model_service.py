import json
import os
from typing import Any

from dotenv import load_dotenv
from openai import AsyncOpenAI

from app.models import DeepAnalysisResult, ResolvePilotHandoff


class GitHubModelService:
    """Calls GitHub Models through the OpenAI-compatible API."""

    def __init__(self) -> None:
        load_dotenv()
        self.api_key = os.getenv("GITHUB_MODELS_API_KEY")
        self.endpoint = os.getenv("GITHUB_MODELS_ENDPOINT", "https://models.github.ai/inference")
        self.model = os.getenv("GITHUB_MODELS_MODEL", "openai/gpt-4.1-mini")

        self.client = AsyncOpenAI(api_key=self.api_key, base_url=self.endpoint) if self.api_key else None

    async def analyze(self, handoff: ResolvePilotHandoff) -> DeepAnalysisResult:
        """Generate deep analysis from already-completed tool results."""

        if self.client is None:
            # A small local fallback keeps local development friendly when .env is absent.
            return self._fallback_analysis(handoff)

        response = await self.client.chat.completions.create(
            model=self.model,
            temperature=0.2,
            response_format={"type": "json_object"},
            messages=[
                {
                    "role": "system",
                    "content": (
                        "You are ResolvePilot.WorkflowEngine. Analyze only the provided "
                        "completed toolExecutionResults. Do not call tools, invent tool "
                        "outputs, create Jira tickets, assign tickets, or send email. "
                        "Return JSON with analysisSummary, actionItems, and confidence."
                    ),
                },
                {
                    "role": "user",
                    "content": json.dumps(self._handoff_for_prompt(handoff), indent=2),
                },
            ],
        )

        content = response.choices[0].message.content or "{}"
        parsed = json.loads(content)

        return DeepAnalysisResult(
            analysisSummary=str(parsed.get("analysisSummary") or handoff.resolutionSummary),
            actionItems=[str(item) for item in parsed.get("actionItems", [])],
            confidence=float(parsed.get("confidence", handoff.confidence)),
        )

    def _handoff_for_prompt(self, handoff: ResolvePilotHandoff) -> dict[str, Any]:
        """Create a JSON-safe prompt payload."""

        if hasattr(handoff, "model_dump"):
            return handoff.model_dump()
        return handoff.dict()

    def _fallback_analysis(self, handoff: ResolvePilotHandoff) -> DeepAnalysisResult:
        """Simple fallback used only when GitHub Models is not configured."""

        findings = handoff.investigationFindings
        evidence = "; ".join(findings.evidence) if findings else "No evidence provided."
        root_cause = findings.rootCauseHypothesis if findings else "No root cause hypothesis provided."
        summary = (
            f"{handoff.resolutionSummary} Root cause hypothesis: "
            f"{root_cause}. Evidence: {evidence}"
        )
        return DeepAnalysisResult(
            analysisSummary=summary,
            actionItems=[handoff.recommendedAction],
            confidence=handoff.confidence,
        )
