using Asp.Versioning;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using SuitStore.Alterations.Core.Messages;

namespace SuitStore.Alterations.Api.Endpoints.Alterations;

[ApiController]
[ApiVersion("1")]
[Route("v{version:apiVersion}/alterations")]
[Produces("application/json")]
public class Finish(IPublishEndpoint publishEndpoint) : ControllerBase
{
    [HttpPost("{alterationId}/finish")]
    public async Task<ActionResult> Execute(Guid alterationId, CancellationToken cancellationToken)
    {
        // Validate if tailor exists
        
        // TOOD: return not found if no alteration with Id found
        await publishEndpoint.Publish(new AlterationFinished(alterationId), cancellationToken);

        return Ok();
    }
}