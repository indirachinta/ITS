using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using ResolvePilot.Application.ToolCortex;
using ResolvePilot.Domain.ToolCortex;

namespace ResolvePilot.Infrastructure.ToolCortex;

public sealed class HttpToolCortexClient(HttpClient httpClient) : IToolCortexClient
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() }
    };

    public async Task<ToolCortexResponse> SelectToolsAsync(
        ToolCortexRequest request,
        CancellationToken cancellationToken = default)
    {
        HttpResponseMessage response;

        try
        {
            response = await httpClient.PostAsJsonAsync(
                "/mcp/selecttools",
                request,
                JsonOptions,
                cancellationToken);
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or InvalidOperationException)
        {
            throw new InvalidOperationException(
                "ToolCortex HTTP integration failed while calling POST /mcp/selecttools. Verify ToolCortex:BaseUrl and that the ToolCortex API is running.",
                ex);
        }

        if (!response.IsSuccessStatusCode)
        {
            string errorBody = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new InvalidOperationException(
                $"ToolCortex HTTP integration failed with status {(int)response.StatusCode} ({response.ReasonPhrase}) from POST /mcp/selecttools. Body: {errorBody}");
        }

        ToolCortexResponse? toolCortexResponse = await response.Content.ReadFromJsonAsync<ToolCortexResponse>(
            JsonOptions,
            cancellationToken);

        return toolCortexResponse ?? throw new InvalidOperationException(
            "ToolCortex HTTP integration returned an empty or invalid response body.");
    }
}
