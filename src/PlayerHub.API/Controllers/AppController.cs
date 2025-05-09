using Core.Wrappers;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;

namespace PlayerHub.API.Controllers
{
    /// <summary>
    /// Base controller providing common functionality for API controllers.
    /// </summary>
    [ApiController]
    [Produces("application/json")]
    public class AppController : ControllerBase
    {
        /// <summary>
        /// Generates an appropriate HTTP response based on the provided <see cref="IResponse"/>.
        /// </summary>
        /// <param name="response">The response object containing the status code and optional data or error message.</param>
        /// <returns>An <see cref="IActionResult"/> representing the HTTP response.</returns>
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

        /// <summary>
        /// Gets the email address of the currently authenticated user from their claims.
        /// </summary>
        protected string IdentityEmail =>
            User?.Claims.Where(x => x.Type == ClaimTypes.Email)
                .Select(x => x.Value)
                .FirstOrDefault() ?? string.Empty;
    }
}
