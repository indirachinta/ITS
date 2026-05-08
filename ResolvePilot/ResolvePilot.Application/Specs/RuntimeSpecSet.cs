namespace ResolvePilot.Application.Specs;

public sealed record RuntimeSpecSet(IReadOnlyList<RuntimeSpec> Specs)
{
    public string ToPromptText()
    {
        return string.Join(
            Environment.NewLine + Environment.NewLine,
            Specs.Select(spec => $"### {spec.Name}{Environment.NewLine}{spec.Content}"));
    }
}
