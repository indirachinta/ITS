using Microsoft.Extensions.DependencyInjection;
using ResolvePilot.Application.Prompts;
using ResolvePilot.Application.ToolCortex;

namespace ResolvePilot.Application;

public static class ResolvePilotApplicationServices
{
    public static IServiceCollection AddResolvePilotApplication(this IServiceCollection services)
    {
        services.AddSingleton<IPromptBuilder, PromptBuilder>();
        services.AddSingleton<IToolCortexClient, MockToolCortexClient>();
        services.AddSingleton<PathDecisionService>();
        services.AddSingleton<IIncidentResolutionEngine, IncidentResolutionEngine>();

        return services;
    }
}
