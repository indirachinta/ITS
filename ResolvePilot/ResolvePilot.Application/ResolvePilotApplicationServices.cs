using Microsoft.Extensions.DependencyInjection;
using IncidentTools.Core;
using ResolvePilot.Application.Prompts;

namespace ResolvePilot.Application;

public static class ResolvePilotApplicationServices
{
    public static IServiceCollection AddResolvePilotApplication(this IServiceCollection services)
    {
        services.AddSingleton<IPromptBuilder, PromptBuilder>();
        services.AddIncidentToolsCore();
        services.AddSingleton<ExternalMcpToolExecutionPlaceholder>();
        services.AddSingleton<PathDecisionService>();
        services.AddSingleton<IIncidentResolutionEngine, IncidentResolutionEngine>();

        return services;
    }
}
