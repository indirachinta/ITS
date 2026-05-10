# V3 — FAST Resolution Spec

## Purpose

V3 introduces FAST-path tool execution into ResolvePilot.

ResolvePilot should:
- identify FAST incidents
- execute lightweight recommended tools
- aggregate evidence
- synthesize structured response

## Existing Foundation

V1:
- project structure
- runtime specs folder
- basic resolve endpoint

V2:
- Microsoft.Extensions.AI integration
- GitHub Models experimentation
- runtime spec-governed prompts
- ToolCortex integration contract
- FAST/DEEP orchestration

## FAST Resolution

FAST resolution is used when:
- confidence is high
- issue scope is isolated
- deterministic execution is enough
- limited tools are sufficient

Examples:
- deployment regression
- isolated runtime failure
- health degradation

## FAST Execution Flow

ResolvePilot:
- receives ToolCortex recommendations
- decides FAST
- executes lightweight tools
- aggregates evidence
- returns structured response

## DEEP Resolution

DEEP path means:
- no tools executed
- workflowRequired = true
- future workflow boundary

ResolvePilot does not implement DEEP workflows.

Future deep workflows belong to Component 3.