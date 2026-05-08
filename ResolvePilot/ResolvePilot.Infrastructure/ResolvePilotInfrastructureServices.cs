using System.ClientModel;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenAI;
using ResolvePilot.Application.Ai;
using ResolvePilot.Application.Specs;
using ResolvePilot.Infrastructure.Ai;

namespace ResolvePilot.Infrastructure;

public static class ResolvePilotInfrastructureServices
{
    public static IServiceCollection AddResolvePilotInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration,
        string contentRootPath)
    {
        services.AddSingleton<IRuntimeSpecLoader>(_ => new FileSystemRuntimeSpecLoader(FindSpecsPath(contentRootPath)));
        services.AddSingleton<IAiDecisionService, AiDecisionService>();
        services.AddSingleton(BuildChatClient(configuration));

        return services;
    }

    private static IChatClient BuildChatClient(IConfiguration configuration)
    {
        

        IConfigurationSection section = configuration.GetSection("Ai:GitHubModels");

        string? endpoint = section["Endpoint"] ?? "";
        string? model = section["Model"] ?? "";
        string? apiKey = section["ApiKey"] ?? "";

        if (string.IsNullOrWhiteSpace(apiKey))
        {
            throw new InvalidOperationException(
                $"GitHub Models is configured as the first AI provider, but apiKey is not set.");
        }

        OpenAIClientOptions options = new()
        {
            Endpoint = new Uri(endpoint)
        };

        return new OpenAI.Chat.ChatClient(model, new ApiKeyCredential(apiKey), options).AsIChatClient();
    }

    private static string FindSpecsPath(string contentRootPath)
    {
        foreach (string candidate in new[]
        {
            Path.Combine(contentRootPath, "Specs"),
            Path.Combine(contentRootPath, "..", "Specs"),
            Path.Combine(AppContext.BaseDirectory, "Specs")
        })
        {
            string fullPath = Path.GetFullPath(candidate);

            if (Directory.Exists(fullPath))
            {
                return fullPath;
            }
        }

        return Path.GetFullPath(Path.Combine(contentRootPath, "Specs"));
    }
}
