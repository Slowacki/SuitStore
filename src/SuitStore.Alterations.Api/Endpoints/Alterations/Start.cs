using System.Net;
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
    /// <summary>
    /// Indicates the actual, physical alterations process has been started by a tailor
    /// </summary>
    /// <param name="alterationId">Id of the alteration</param>
    /// <param name="request">Request containing the id of the tailor who will undertake the alteration</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("{alterationId}/start")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult> Execute(Guid alterationId, [FromBody] StartAlterationRequest request, CancellationToken cancellationToken)
    {
        // Validate if tailor exists
        
        var result = await requestClient.GetResponse<AlterationStarted, AlterationNotFound>(new StartAlteration(alterationId, request.TailorId), cancellationToken);

        if (result.Message is AlterationNotFound)
            return NotFound();
        
        return Ok();
    }
}