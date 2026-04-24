# Demo Scenarios Spec

## Scenario 1
Title: Checkout API failing intermittently
Summary: Customers report random failures during checkout. Errors increased after a recent deployment.
Affected Service: checkout-service
Incident Type: runtime failure
Symptom Type: intermittent API failure
Severity: high

Expected likely top tools:
- get_service_logs
- get_recent_deployments
- get_service_health

## Scenario 2
Title: Need to inspect service contract and related repo details
Summary: Team is unsure whether the issue is caused by API contract mismatch or documentation gap.
Affected Service: order-service
Incident Type: investigation
Symptom Type: unclear integration issue
Severity: medium

Expected likely top tools:
- api_schema_inspector
- github_repo_search
- get_service_dependencies