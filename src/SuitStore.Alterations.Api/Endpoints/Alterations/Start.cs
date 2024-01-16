using Asp.Versioning;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using SuitStore.Alterations.Api.Requests;
using SuitStore.Alterations.Core.Messages;

namespace SuitStore.Alterations.Api.Endpoints.Alterations;

[ApiController]
[ApiVersion("1")]
[Route("v{version:apiVersion}/alterations")]
[Produces("application/json")]
public class Start(IRequestClient<StartAlteration> requestClient) : ControllerBase
{
    [HttpPost("{alterationId}/start")]
    public async Task<ActionResult> Execute(Guid alterationId, [FromBody] StartAlterationRequest request, CancellationToken cancellationToken)
    {
        // Validate if tailor exists
        
        var result = await requestClient.GetResponse<AlterationStarted, AlterationNotFound>(new StartAlteration(alterationId, request.TailorId), cancellationToken);

        if (result.Message is AlterationNotFound)
            return NotFound();
        
        return Ok();
    }
}