# Response Contract Spec

## Selector Response

{
  "incidentSummary": "string",
  "recommendedTools": [
    {
      "toolName": "string",
      "sourceType": "custom | external",
      "score": 0.0,
      "confidence": 0.0,
      "reason": "string"
    }
  ],
  "rejectedTools": [
    {
      "toolName": "string",
      "sourceType": "custom | external",
      "reason": "string"
    }
  ]
}

## Rules
- recommendedTools must be sorted descending by score
- top recommendations should be limited to 3
- each recommendation must include reason
- sourceType must always be present
- rejectedTools are optional but useful for explainability