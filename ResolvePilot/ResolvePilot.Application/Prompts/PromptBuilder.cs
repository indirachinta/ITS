using System.Text.Json;
using ResolvePilot.Application.Specs;
using ResolvePilot.Domain;

namespace ResolvePilot.Application.Prompts;

public sealed class PromptBuilder : IPromptBuilder
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = true
    };

    public string Build(IncidentRequest request, RuntimeSpecSet runtimeSpecs)
    {
        string incidentJson = JsonSerializer.Serialize(request, JsonOptions);

        return $$"""
            You are ResolvePilot V2. Make a spec-governed incident decision.

            Use the runtime specs exactly as policy inputs. Do not execute tools. Do not invent ToolCortex scoring.

            Runtime specs:
            {{runtimeSpecs.ToPromptText()}}

            Incident request:
            {{incidentJson}}

            Return only valid JSON matching this contract:
            {
              "incidentIntent": "deployment_related_failure | single_service_runtime_failure | dependency_failure | data_issue | unknown",
              "affectedService": "string",
              "resolutionSummary": "string",
              "recommendedAction": "string",
              "confidence": 0.0,
              "reasoning": ["string"],
              "decisionSource": "runtime-spec-governed-ai-github-models"
            
            }
            """;
    }
}
