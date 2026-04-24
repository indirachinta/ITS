using ToolCortex.Domain;

namespace ToolCortex.Application;

public sealed class DeterministicToolSelectionEngine(IToolRegistry registry) : IToolSelectionEngine
{
    public SelectorResponse SelectTools(IncidentRequest request)
    {
        var signals = AnalyzeSignals(request);
        var evaluated = registry.GetAllTools()
            .Select(tool => EvaluateTool(tool, request, signals))
            .ToList();

        var recommended = evaluated
            .Where(e => e.IsRecommended)
            .OrderByDescending(e => e.Score)
            .ThenByDescending(e => e.SpecificityTieBreaker)
            .ThenByDescending(e => e.CustomTieBreaker)
            .ThenByDescending(e => e.OperationalSimplicityTieBreaker)
            .ThenBy(e => e.Tool.ToolName, StringComparer.Ordinal)
            .Take(3)
            .Select(e => new ToolRecommendation(
                e.Tool.ToolName,
                e.Tool.SourceType,
                e.Score,
                e.Confidence,
                e.Reason))
            .ToList();

        var rejected = evaluated
            .Where(e => !e.IsRecommended)
            .Select(e => new RejectedTool(e.Tool.ToolName, e.Tool.SourceType, e.Reason))
            .ToList();

        return new SelectorResponse(
            $"{request.Title} | {request.IncidentType} | {request.SymptomType} | {request.Severity}",
            recommended,
            rejected.Count == 0 ? null : rejected);
    }

    private static SignalAnalysis AnalyzeSignals(IncidentRequest request)
    {
        var text = $"{request.Title} {request.Summary} {request.IncidentType} {request.SymptomType}".ToLowerInvariant();
        var intents = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        var hasRuntimeSignal =
            text.Contains("runtime") || text.Contains("failure") || text.Contains("exception") ||
            text.Contains("timeout") || text.Contains("intermittent") || text.Contains("error");
        if (hasRuntimeSignal)
        {
            intents.Add("logs");
            intents.Add("health");
            intents.Add("incident-details");
        }

        var hasDeploymentSignal =
            text.Contains("deployment") || text.Contains("release") || text.Contains("regression") ||
            text.Contains("after-change") || text.Contains("recent rollout") || text.Contains("recent deploy");
        if (hasDeploymentSignal)
        {
            intents.Add("deployments");
        }

        var hasSchemaSignal =
            text.Contains("contract mismatch") || text.Contains("api mismatch") || text.Contains("schema issue") ||
            text.Contains("interface mismatch") || text.Contains("service contract") || text.Contains("contract") ||
            text.Contains("schema");
        if (hasSchemaSignal)
        {
            intents.Add("schema");
        }

        var hasRepoDocsSignal = text.Contains("repo") || text.Contains("documentation") || text.Contains("docs");
        if (hasRepoDocsSignal)
        {
            intents.Add("repo-search");
            intents.Add("docs");
        }

        if (text.Contains("integration") || text.Contains("dependency"))
        {
            intents.Add("dependencies");
        }

        var hasHealthSignal =
            text.Contains("outage") || text.Contains("degraded") || text.Contains("unavailable") ||
            text.Contains("instability") || text.Contains("health issue") || text.Contains("service down");
        if (hasHealthSignal)
        {
            intents.Add("health");
        }

        var hasMissingContextSignal = IsLikelyMissingContext(request);
        if (hasMissingContextSignal)
        {
            intents.Add("incident-details");
        }

        if (intents.Count == 0)
        {
            intents.Add("incident-details");
        }

        return new SignalAnalysis(
            intents,
            hasRuntimeSignal,
            hasDeploymentSignal,
            hasSchemaSignal,
            hasHealthSignal,
            hasMissingContextSignal);
    }

    private static Evaluation EvaluateTool(ToolDefinition tool, IncidentRequest request, SignalAnalysis signals)
    {
        var score = tool.PriorityWeight * 0.35;
        var reasons = new List<string>();
        var isDisallowed = false;
        var joinedContext = $"{request.Title} {request.Summary} {request.SymptomType} {request.IncidentType}".ToLowerInvariant();

        var matchedIntents = tool.SupportedIntents.Where(i => signals.Intents.Contains(i)).ToList();
        if (matchedIntents.Count > 0)
        {
            score += 0.22;
            reasons.Add($"Strong intent alignment with {string.Join(", ", matchedIntents)} investigation needs");
        }
        else
        {
            score -= 0.10;
            reasons.Add("Limited direct alignment with the primary investigation intent");
        }

        if (IsSpecificTool(tool))
        {
            score += 0.12;
            reasons.Add("Preferred because it is a direct operational or specialized diagnostic tool");
        }
        else
        {
            score += 0.02;
            reasons.Add("Useful supporting tool, but less specific than top diagnostic options");
        }

        if (signals.HasDeploymentSignal &&
            tool.SupportedIntents.Contains("deployments", StringComparer.OrdinalIgnoreCase))
        {
            score += 0.26;
            reasons.Add("Boosted because the incident explicitly mentions a recent deployment or release change");
        }

        if (signals.HasSchemaSignal &&
            tool.SupportedIntents.Contains("schema", StringComparer.OrdinalIgnoreCase))
        {
            score += 0.32;
            reasons.Add("Ranked high because schema or contract mismatch is central to the request");
        }
        else if (signals.HasSchemaSignal &&
                 (tool.SupportedIntents.Contains("repo-search", StringComparer.OrdinalIgnoreCase) ||
                  tool.SupportedIntents.Contains("docs", StringComparer.OrdinalIgnoreCase)))
        {
            score += 0.10;
            reasons.Add("Helpful secondary path for contract-related investigation through repo/docs evidence");
        }

        if (signals.HasRuntimeSignal &&
            tool.SupportedIntents.Contains("logs", StringComparer.OrdinalIgnoreCase))
        {
            score += 0.28;
            reasons.Add("Strong match for runtime failure and intermittent error analysis");
        }

        if (signals.HasRuntimeSignal &&
            tool.SupportedIntents.Contains("health", StringComparer.OrdinalIgnoreCase))
        {
            score += 0.18;
            reasons.Add("Important for validating service stability while runtime failures are active");
        }

        if (signals.HasHealthSignal &&
            tool.SupportedIntents.Contains("health", StringComparer.OrdinalIgnoreCase))
        {
            score += 0.22;
            reasons.Add("Prioritized because the request indicates potential outage or degraded health");
        }

        if (signals.HasMissingContextSignal &&
            tool.SupportedIntents.Contains("incident-details", StringComparer.OrdinalIgnoreCase))
        {
            score += 0.20;
            reasons.Add("Boosted to enrich missing incident context before deeper diagnosis");
        }

        var preferredMatches = tool.PreferredScenarios.Count(s => joinedContext.Contains(s.ToLowerInvariant()));
        if (preferredMatches > 0)
        {
            var preferredBoost = Math.Min(0.16, preferredMatches * 0.05);
            score += preferredBoost;
            reasons.Add($"Aligned with {preferredMatches} preferred investigation scenario signal(s)");
        }

        if (tool.DisallowedScenarios.Any(s => joinedContext.Contains(s.ToLowerInvariant())))
        {
            isDisallowed = true;
            score -= 0.40;
            reasons.Add("De-prioritized because this tool is not suitable for the detected scenario");
        }

        score = Math.Clamp(score, 0.0, 1.0);
        var confidence = Math.Clamp(0.35 + (score * 0.65), 0.0, 1.0);

        var specificityTieBreaker = IsSpecificTool(tool) ? 1 : 0;
        var customTieBreaker = tool.SourceType == ToolSourceType.Custom ? 1 : 0;
        var operationalSimplicityTieBreaker = IsOperationalTool(tool) ? 1 : 0;
        var isRecommended = !isDisallowed && score >= 0.45;
        var reason = string.Join(" | ", reasons);

        return new Evaluation(
            tool,
            score,
            confidence,
            isRecommended,
            specificityTieBreaker,
            customTieBreaker,
            operationalSimplicityTieBreaker,
            reason);
    }

    private static bool IsLikelyMissingContext(IncidentRequest request)
    {
        var shortSummary = string.IsNullOrWhiteSpace(request.Summary) || request.Summary.Trim().Length < 30;
        var missingType =
            string.IsNullOrWhiteSpace(request.IncidentType) ||
            request.IncidentType.Contains("unknown", StringComparison.OrdinalIgnoreCase) ||
            request.IncidentType.Contains("n/a", StringComparison.OrdinalIgnoreCase);
        var missingSymptom =
            string.IsNullOrWhiteSpace(request.SymptomType) ||
            request.SymptomType.Contains("unknown", StringComparison.OrdinalIgnoreCase) ||
            request.SymptomType.Contains("n/a", StringComparison.OrdinalIgnoreCase);

        // Treat as missing context only when key fields are genuinely sparse.
        return shortSummary && (missingType || missingSymptom);
    }

    private static bool IsSpecificTool(ToolDefinition tool) =>
        tool.ToolName is "get_service_logs" or "get_recent_deployments" or "get_service_health" or
            "api_schema_inspector" or "get_service_dependencies";

    private static bool IsOperationalTool(ToolDefinition tool) =>
        tool.ToolName is "get_service_logs" or "get_recent_deployments" or "get_service_health";

    private sealed record SignalAnalysis(
        IReadOnlySet<string> Intents,
        bool HasRuntimeSignal,
        bool HasDeploymentSignal,
        bool HasSchemaSignal,
        bool HasHealthSignal,
        bool HasMissingContextSignal);

    private sealed record Evaluation(
        ToolDefinition Tool,
        double Score,
        double Confidence,
        bool IsRecommended,
        int SpecificityTieBreaker,
        int CustomTieBreaker,
        int OperationalSimplicityTieBreaker,
        string Reason);
}
