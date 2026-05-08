using Microsoft.AspNetCore.Mvc;
using ResolvePilot.Application;
using ResolvePilot.Domain;

namespace ResolvePilot.Api.Controllers;

[ApiController]
[Route("api/incidents")]
public sealed class IncidentsController(IIncidentResolutionEngine resolutionEngine) : ControllerBase
{
    [HttpPost("resolve")]
    [ProducesResponseType<ResolutionResponse>(StatusCodes.Status200OK)]
    public async Task<ActionResult<ResolutionResponse>> ResolveAsync(
        IncidentRequest request,
        CancellationToken cancellationToken)
    {
        ResolutionResponse response = await resolutionEngine.ResolveAsync(request, cancellationToken);

        return Ok(response);
    }
}
