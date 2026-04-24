# Tool Universe Spec

## Objective
Define the complete set of tools that the Intelligent MCP Tool Selector can evaluate.

## Tool Categories

### A. Custom Tools
These are project-owned enterprise-style tools implemented within this solution.

Examples:
1. get_incident_details
2. get_service_logs
3. get_recent_deployments
4. get_service_dependencies
5. get_service_health

### B. External/Open-Source MCP Tools
These are MCP-compatible tools not owned by this project but represented through metadata for selection purposes.

Examples:
1. fetch_docs_search
2. github_repo_search
3. log_query_generic
4. api_schema_inspector
5. file_content_lookup

## Rule
The selector must treat both categories through a common metadata model.

## Required Metadata per Tool
- tool name
- source type (custom or external)
- purpose
- supported intents
- required inputs
- preferred scenarios
- disallowed scenarios
- priority weight
- output summary

## Notes
- v1 does not require live connectivity to real external MCP servers
- external/open-source tools can be mocked as metadata-only definitions
- the important point is that the selector can evaluate a mixed tool ecosystem