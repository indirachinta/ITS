namespace ResolvePilot.Application.Specs;

public sealed class FileSystemRuntimeSpecLoader(string specsPath) : IRuntimeSpecLoader
{
    public async Task<RuntimeSpecSet> LoadAsync(CancellationToken cancellationToken = default)
    {
        if (!Directory.Exists(specsPath))
        {
            throw new DirectoryNotFoundException($"Runtime specs directory was not found: {specsPath}");
        }

        List<RuntimeSpec> specs = [];

        foreach (string file in Directory.EnumerateFiles(specsPath, "*.json").Order())
        {
            string content = await File.ReadAllTextAsync(file, cancellationToken);
            specs.Add(new RuntimeSpec(Path.GetFileName(file), content));
        }

        if (specs.Count == 0)
        {
            throw new InvalidOperationException($"Runtime specs directory contains no JSON specs: {specsPath}");
        }

        return new RuntimeSpecSet(specs);
    }
}
