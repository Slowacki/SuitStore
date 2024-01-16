using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using SuitStore.Alterations.Api.Requests;
using SuitStore.Alterations.Core.Contracts;

namespace SuitStore.Alterations.Api.Endpoints.Alterations;

[ApiController]
[ApiVersion("1")]
[Route("v{version:apiVersion}/alterations")]
[Produces("application/json")]
public class Get(IAlterationsStore alterationsStore) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult> Execute([FromBody] GetAlterationsRequest request, CancellationToken cancellationToken)
    {
        var alterations = await alterationsStore.GetAsync(request.TailorId, request.State, cancellationToken);

        return Ok(alterations);
    }
}