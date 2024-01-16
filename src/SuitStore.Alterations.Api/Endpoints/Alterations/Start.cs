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
public class Start(IPublishEndpoint publishEndpoint) : ControllerBase
{
    [HttpPost("{alterationId}/start")]
    public async Task<ActionResult> Execute(Guid alterationId, [FromBody] StartAlterationRequest request, CancellationToken cancellationToken)
    {
        // Validate if tailor exists
        
        // TOOD: return not found if no alteration with Id found
        await publishEndpoint.Publish(new AlterationStarted(alterationId, request.TailorId), cancellationToken);

        return Ok();
    }
}