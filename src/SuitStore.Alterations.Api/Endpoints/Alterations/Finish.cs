using System.Net;
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
    /// <summary>
    /// Finishes the work on a alteration
    /// </summary>
    /// <param name="alterationId">the id of the alteration to be finished</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("{alterationId}/finish")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult> Execute(Guid alterationId, CancellationToken cancellationToken)
    {
        // Validate if tailor exists
        
        var result = await requestClient.GetResponse<AlterationFinished,AlterationNotFound>(new FinishAlteration(alterationId), cancellationToken);

        if (result.Message is AlterationNotFound)
            return NotFound();
        
        return Ok();
    }
}