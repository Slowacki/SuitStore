using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using SuitStore.Alterations.Core.Contracts;

namespace SuitStore.Alterations.Api.Endpoints.Alterations;

[ApiController]
[ApiVersion("1")]
[Route("v{version:apiVersion}/tailors")]
[Produces("application/json")]
public class GetByTailorId(IAlterationsStore alterationsStore) : ControllerBase
{
    [HttpGet("{tailorId}/alterations")]
    public async Task<ActionResult> Execute(long tailorId, CancellationToken cancellationToken)
    {
        var alterations = await alterationsStore.GetByTailorIdAsync(tailorId, cancellationToken);

        return Ok(alterations);
    }
}