using System.Net;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using SuitStore.Alterations.Api.Requests;
using SuitStore.Alterations.Core.Contracts;
using SuitStore.Alterations.Core.Models;

namespace SuitStore.Alterations.Api.Endpoints.Alterations;

[ApiController]
[ApiVersion("1")]
[Route("v{version:apiVersion}/alterations")]
[Produces("application/json")]
public class Get(IAlterationsStore alterationsStore) : ControllerBase
{
    /// <summary>
    /// Gets all the alterations matching the filter
    /// </summary>
    /// <param name="request">Optional filter containing tailor id and/or state of the alteration</param>
    /// <param name="cancellationToken"></param>
    /// <returns>List of alterations</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Alteration>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult> Execute([FromQuery] GetAlterationsRequest request, CancellationToken cancellationToken)
    {
        var alterations = await alterationsStore.GetAsync(request.TailorId, request.State, cancellationToken);

        return Ok(alterations);
    }
}