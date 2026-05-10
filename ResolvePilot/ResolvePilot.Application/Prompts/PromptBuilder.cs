using System.Text.Json;
using IncidentTools.Core;
using ResolvePilot.Application.Specs;
using ResolvePilot.Domain;
using ResolvePilot.Domain.Ai;
using ResolvePilot.Domain.ToolCortex;

namespace ResolvePilot.Application.Prompts;

public sealed class PromptBuilder : IPromptBuilder
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = true
    };

    public string BuildIncidentUnderstandingPrompt(IncidentRequest request, RuntimeSpecSet runtimeSpecs)
    {
        string incidentJson = JsonSerializer.Serialize(request, JsonOptions);

        return $$"""
            You are ResolvePilot V3. Make a spec-governed incident understanding decision.

            Use the runtime specs exactly as policy inputs. Do not execute tools. Do not invent ToolCortex scoring.
            Do not provide final resolution. Only classify and understand the incident.

            Runtime specs:
            {{runtimeSpecs.ToPromptText()}}

            Incident request:
            {{incidentJson}}

            Return only valid JSON matching this contract:
            {
              "incidentIntent": "deployment_related_failure | single_service_runtime_failure | dependency_failure | data_issue | unknown",
              "affectedService": "string",
              "confidence": 0.0,
              "reasoning": ["string"],
              "decisionSource": "runtime-spec-governed-ai-github-models"
            }
            """;
    }

    public string BuildFinalResolutionPrompt(
        IncidentRequest request,
        AiIncidentUnderstanding incidentUnderstanding,
        ToolCortexResponse toolCortexResponse,
        IReadOnlyList<ToolExecutionResult> toolExecutionResults,
        RuntimeSpecSet runtimeSpecs)
    {
        string incidentJson = JsonSerializer.Serialize(request, JsonOptions);
        string understandingJson = JsonSerializer.Serialize(incidentUnderstanding, JsonOptions);
        string toolCortexJson = JsonSerializer.Serialize(toolCortexResponse, JsonOptions);
        string toolResultsJson = JsonSerializer.Serialize(toolExecutionResults, JsonOptions);

        return $$"""
            You are ResolvePilot V3. Produce the final user-facing FAST resolution.

            Use only the provided tool evidence. Do not invent facts. Produce final user-facing resolution.
            Runtime specs are policy inputs and must remain part of your decision context.

            Runtime specs:
            {{runtimeSpecs.ToPromptText()}}

            Original incident request:
            {{incidentJson}}

            AI incident understanding:
            {{understandingJson}}

            ToolCortex response:
            {{toolCortexJson}}

            Tool execution results:
            {{toolResultsJson}}

            Return only valid JSON matching this contract:
            {
              "resolutionSummary": "string",
              "recommendedAction": "string",
              "confidence": 0.0,
              "reasoning": ["string"]
            }
            """;
    }
}
