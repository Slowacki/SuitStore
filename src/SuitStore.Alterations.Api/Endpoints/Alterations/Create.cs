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
    [HttpPost]
    public async Task<ActionResult> Execute(long clientId, [FromBody] CreateAlterationRequest alterationRequest, CancellationToken cancellationToken)
    {
        if (alterationRequest.AlterationInstructions.AlterationInstruction.Any(a => a.ChangeInCm is > 5 or < -5))
            return BadRequest("Cannot alter length by more than 5 cm.");
        
        if (alterationRequest.AlterationInstructions.AlterationInstruction.DistinctBy(a => a.Type).Count() != alterationRequest.AlterationInstructions.AlterationInstruction.Count())
            return BadRequest("Invalid alteration instructions.");

        // Validate client and product exist
        
        var request = new CreateAlteration(clientId, alterationRequest.ProductId, alterationRequest.AlterationInstructions);

        await requestClient.GetResponse<AlterationCreated>(request, cancellationToken);

        return Ok();
    }
}