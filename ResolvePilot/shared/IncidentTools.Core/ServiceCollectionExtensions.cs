using Microsoft.Extensions.DependencyInjection;
using IncidentTools.Core.Tools;

namespace IncidentTools.Core;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddIncidentToolsCore(this IServiceCollection services)
    {
        services.AddSingleton<IIncidentTool, ServiceLogsTool>();
        services.AddSingleton<IIncidentTool, RecentDeploymentsTool>();
        services.AddSingleton<IIncidentTool, ServiceHealthTool>();
        services.AddSingleton<IIncidentTool, ServiceDependenciesTool>();
        services.AddSingleton<IToolExecutionService, ToolExecutionService>();

        return services;
    }
}
