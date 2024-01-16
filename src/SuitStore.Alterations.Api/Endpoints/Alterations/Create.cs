using System.Net;
using Asp.Versioning;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using SuitStore.Alterations.Api.Requests;
using SuitStore.Alterations.Core.Messages;
using SuitStore.Alterations.Core.Models;

namespace SuitStore.Alterations.Api.Endpoints.Alterations;

[ApiController]
[ApiVersion("1")]
[Route("v{version:apiVersion}/clients/{clientId}/alterations")]
[Produces("application/json")]
public class Create(IRequestClient<CreateAlteration> requestClient) : ControllerBase
{
    /// <summary>
    /// Creates a new alteration
    /// </summary>
    /// <param name="clientId">Id of the client submitting a product to alter</param>
    /// <param name="alterationRequest">Request containing alteration instructions and product id</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<ActionResult> Execute(long clientId, [FromBody] CreateAlterationRequest alterationRequest, CancellationToken cancellationToken)
    {
        if (alterationRequest.AlterationInstructions.Any(a => a.ChangeInCm is > 5 or < -5))
            return BadRequest("Cannot alter length by more than 5 cm.");
        
        if (alterationRequest.AlterationInstructions.DistinctBy(a => a.Type).Count() != alterationRequest.AlterationInstructions.Count())
            return BadRequest("Invalid alteration instructions.");

        // Validate client and product exist
        
        var request = new CreateAlteration(clientId, alterationRequest.ProductId, alterationRequest.AlterationInstructions);

        await requestClient.GetResponse<AlterationCreated>(request, cancellationToken);

        return Ok();
    }
}