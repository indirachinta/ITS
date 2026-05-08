namespace ResolvePilot.Application.Specs;

public interface IRuntimeSpecLoader
{
    Task<RuntimeSpecSet> LoadAsync(CancellationToken cancellationToken = default);
}
