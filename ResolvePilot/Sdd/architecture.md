POST /api/incidents/resolve
        ↓
ResolvePilot API
        ↓
Runtime Specs + Microsoft.Extensions.AI
        ↓
GitHub Models
        ↓
AI Incident Decision
        ↓
HTTP call to ToolCortex API
        ↓
Ranked recommendations
        ↓
PathDecisionService
        ↓
FAST or DEEP
   ┌───────────────┬────────────────────┐
   │ FAST          │ DEEP               │
   ↓               ↓
Tool execution     Structured handoff
router             for Component 3
   ↓
Custom tool?
   ├── Yes → IncidentTools.Core executes
   └── No  → External MCP placeholder result