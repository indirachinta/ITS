using ResolvePilot.Application;
using ResolvePilot.Domain;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IIncidentResolutionEngine, IncidentResolutionEngine>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
// TODO Day 2: Register infrastructure services such as RuntimeSpecLoader,
// ToolCortexClient, and future LLM decision service.

app.MapPost("/api/incidents/resolve", (
    IncidentRequest request,
    IIncidentResolutionEngine resolutionEngine) =>
{
    ResolutionResponse response = resolutionEngine.Resolve(request);

    return Results.Ok(response);
})
.WithName("ResolveIncident")
.WithSummary("Resolve an incident")
.WithDescription("Returns a structured mock FAST resolution response for the supplied incident request.")
.Accepts<IncidentRequest>("application/json")
.Produces<ResolutionResponse>(StatusCodes.Status200OK);

app.Run();
