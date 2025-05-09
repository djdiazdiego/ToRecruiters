using Core.Wrappers;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;

namespace PlayerHub.API.Controllers
{
    [ApiController]
    [Produces("application/json")]
    public class AppController : ControllerBase
    {
        protected IActionResult GenerateResponse(IResponse response)
        {
            return response.Code switch
            {
                (int)HttpStatusCode.OK => Ok(response),
                (int)HttpStatusCode.BadRequest => BadRequest(response),
                (int)HttpStatusCode.Unauthorized => Unauthorized(response),
                (int)HttpStatusCode.Forbidden => Forbid(),
                (int)HttpStatusCode.NotFound => NotFound(response),
                (int)HttpStatusCode.Conflict => Conflict(response),
                _ => Ok(Core.Wrappers.Response.Ok),
            };
        }

        protected string IdentityEmail =>
            User?.Claims.Where(x => x.Type == ClaimTypes.Email)
                .Select(x => x.Value)
                .FirstOrDefault() ?? string.Empty;
    }
}
