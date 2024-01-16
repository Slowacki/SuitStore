using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace SuitStore.Alterations.Api.Endpoints.Alterations;

[ApiController]
[ApiVersion("1")]
[Route("v{version:apiVersion}/alterations")]
[Produces("application/json")]
public class Create : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult> Execute(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}