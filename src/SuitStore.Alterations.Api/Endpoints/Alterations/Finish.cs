using Asp.Versioning;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using SuitStore.Alterations.Core.Messages;

namespace SuitStore.Alterations.Api.Endpoints.Alterations;

[ApiController]
[ApiVersion("1")]
[Route("v{version:apiVersion}/alterations")]
[Produces("application/json")]
public class Finish(IRequestClient<FinishAlteration> requestClient) : ControllerBase
{
    [HttpPost("{alterationId}/finish")]
    public async Task<ActionResult> Execute(Guid alterationId, CancellationToken cancellationToken)
    {
        // Validate if tailor exists
        
        var result = await requestClient.GetResponse<AlterationFinished,AlterationNotFound>(new FinishAlteration(alterationId), cancellationToken);

        if (result.Message is AlterationNotFound)
            return NotFound();
        
        return Ok();
    }
}