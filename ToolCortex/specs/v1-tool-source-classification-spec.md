# Tool Source Classification Spec

## Purpose
Distinguish where a tool comes from while keeping the selection engine source-agnostic in evaluation.

## Source Types

### 1. Custom
Tools owned and implemented inside this project.

Characteristics:
- directly controlled
- easier to trust
- tailored for enterprise scenarios
- strongly aligned to internal domain

### 2. External
Open-source or third-party MCP-compatible tools represented through metadata.

Characteristics:
- reusable
- broader coverage
- useful for generic tasks
- lower domain specificity in some cases

## Selection Principle
The selector should not automatically prefer custom tools only because they are internal.
The selector should evaluate tools based on context relevance first.

## Optional Bias Rule for v1
If two tools are nearly equal in score and one is a custom enterprise-aligned tool, prefer the custom tool slightly for determinism.

## Non-Goal
This classification is not for security enforcement in v1.
It is mainly for ranking transparency and future extensibility.