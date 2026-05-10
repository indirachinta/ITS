using System.Text.Json;

namespace IncidentTools.Core.Tools;

internal static class MockToolDataReader
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public static async Task<JsonDocument> ReadAsync(
        string fileName,
        CancellationToken cancellationToken = default)
    {
        string path = ResolvePath(fileName);
        await using FileStream stream = File.OpenRead(path);

        return await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);
    }

    public static IReadOnlyDictionary<string, object?> ToRawData(JsonElement payload)
    {
        return new Dictionary<string, object?>
        {
            ["source"] = "mock_json_stream",
            ["payload"] = payload.Clone()
        };
    }

    public static T Deserialize<T>(JsonElement element)
    {
        return element.Deserialize<T>(JsonOptions)
            ?? throw new InvalidOperationException($"Unable to deserialize mock tool payload as {typeof(T).Name}.");
    }

    private static string ResolvePath(string fileName)
    {
        foreach (string candidate in new[]
        {
            Path.Combine(AppContext.BaseDirectory, "Tools", "Mocks", fileName),
            Path.Combine(AppContext.BaseDirectory, "shared", "IncidentTools.Core", "Tools", "Mocks", fileName),
            Path.Combine(Directory.GetCurrentDirectory(), "Tools", "Mocks", fileName),
            Path.Combine(Directory.GetCurrentDirectory(), "..", "shared", "IncidentTools.Core", "Tools", "Mocks", fileName),
            Path.Combine(Directory.GetCurrentDirectory(), "shared", "IncidentTools.Core", "Tools", "Mocks", fileName)
        })
        {
            string fullPath = Path.GetFullPath(candidate);

            if (File.Exists(fullPath))
            {
                return fullPath;
            }
        }

        throw new FileNotFoundException($"Mock tool data file '{fileName}' was not found.");
    }
}
