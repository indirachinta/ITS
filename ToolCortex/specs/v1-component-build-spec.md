# Project:ToolCortex
# Description: Intelligent MCP Tool Selector

## Purpose
Build a .NET-based tool intelligence component that selects and ranks the most relevant tools for an incident or investigation context.

## Why this project exists
Most AI systems either call tools blindly or only consider tools exposed within their own runtime. This project introduces an explicit tool selection layer that can evaluate both custom enterprise tools and external/open-source MCP tools in a structured, explainable way.

## Primary Goal
Given an incident or problem context, return the most relevant tools in ranked order with confidence and reasoning.

## Key Architectural Idea
Tool selection is treated as a separate system concern, not as an incidental LLM behavior.

## Tool Types in Scope
1. Custom tools owned by this project
2. External/open-source MCP tools that can be represented through metadata

## In Scope
- .NET 8 project
- MCP-friendly architecture
- custom mock enterprise tools
- external/open-source MCP tool metadata representation
- deterministic selection rules
- ranked explainable output
- clean demo-ready architecture

## Out of Scope
- real enterprise integrations
- actual external MCP server connectivity for v1
- dynamic tool discovery across network
- agent orchestration
- LangGraph integration
- production deployment

## Input
Problem context such as:
- title
- summary
- affected service
- incident type
- symptom type
- severity
- optional environment

## Output
A ranked list of candidate tools with:
- tool name
- tool source
- score
- confidence
- reason

## Success Criteria
- project runs locally
- selector can evaluate both custom and external/open-source tool definitions
- output is structured and explainable
- architecture is small but senior-looking
- suitable to present as a standalone LinkedIn project