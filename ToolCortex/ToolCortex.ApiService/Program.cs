using ToolCortex.Domain;
using ToolCortex.McpAdapter;
using ToolCortex.Application;
using ToolCortex.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<IMcpToolSelectionFacade, McpToolSelectionEndpoint>();
builder.Services.AddSingleton<IToolSelectionEngine, DeterministicToolSelectionEngine>();
builder.Services.AddSingleton<IToolRegistry, InMemoryToolRegistry>();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();



app.MapPost("/mcp/selecttools", (IMcpToolSelectionFacade mcpToolSelectionFacade, IncidentRequest incidentRequest) =>
{
    if (incidentRequest == null)
        incidentRequest = new IncidentRequest(
        "Checkout API failing intermittently",
        "Customers report random failures during checkout. Errors increased after a recent deployment.",
        "checkout-service",
        "runtime failure",
        "intermittent API failure",
        "high");

    var Tools = mcpToolSelectionFacade.RecommendTools(incidentRequest);
    return Results.Ok(Tools);

});

app.Run();


