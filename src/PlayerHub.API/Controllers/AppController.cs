using Microsoft.AspNetCore.Mvc;
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
        /// Gets the email address of the currently authenticated user from their claims.
        /// </summary>
        protected string IdentityEmail =>
            User?.Claims.Where(x => x.Type == ClaimTypes.Email)
                .Select(x => x.Value)
                .FirstOrDefault() ?? string.Empty;
    }
}
