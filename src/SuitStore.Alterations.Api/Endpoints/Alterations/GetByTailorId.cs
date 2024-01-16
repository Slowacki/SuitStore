using System.Net;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using SuitStore.Alterations.Core.Contracts;
using SuitStore.Alterations.Core.Models;

namespace SuitStore.Alterations.Api.Endpoints.Alterations;

[ApiController]
[ApiVersion("1")]
[Route("v{version:apiVersion}/tailors")]
[Produces("application/json")]
public class GetByTailorId(IAlterationsStore alterationsStore) : ControllerBase
{
    /// <summary>
    /// Gets alterations that are being worked on by the tailor
    /// </summary>
    /// <param name="tailorId">id of the tailor</param>
    /// <param name="cancellationToken"></param>
    /// <returns>List of alterations</returns>
    [HttpGet("{tailorId}/alterations")]
    [ProducesResponseType(typeof(IEnumerable<Alteration>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult> Execute(long tailorId, CancellationToken cancellationToken)
    {
        var alterations = await alterationsStore.GetByTailorIdAsync(tailorId, cancellationToken);

        return Ok(alterations);
    }
}