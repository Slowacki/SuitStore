using Asp.Versioning;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using SuitStore.Alterations.Core.Messages;
using SuitStore.Alterations.Core.Models;

namespace SuitStore.Alterations.Api.Endpoints.Alterations;

[ApiController]
[ApiVersion("1")]
[Route("v{version:apiVersion}/clients/{clientId}/alterations")]
[Produces("application/json")]
public class Create(IPublishEndpoint publishEndpoint) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult> Execute(long clientId, AlterationInstructions instructions, CancellationToken cancellationToken)
    {
        if (instructions.AlterationInstruction.Any(a => a.ChangeInCm is > 5 or < -5))
            return BadRequest("Cannot alter length by more than 5 cm.");
        
        if (instructions.AlterationInstruction.DistinctBy(a => a.Type).Count() != instructions.AlterationInstruction.Count())
            return BadRequest("Invalid alteration instructions.");

        var request = new CreateAlteration(clientId, instructions);

        await publishEndpoint.Publish(request, cancellationToken);

        return Ok();
    }
}