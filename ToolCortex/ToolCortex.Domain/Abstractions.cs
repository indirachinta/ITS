namespace ToolCortex.Domain;

public interface IToolRegistry
{
    IReadOnlyList<ToolDefinition> GetAllTools();
}

public interface IToolSelectionEngine
{
    SelectorResponse SelectTools(IncidentRequest request);
}

public interface IMcpToolSelectionFacade
{
    SelectorResponse RecommendTools(IncidentRequest request);
}
