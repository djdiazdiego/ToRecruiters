using Core.Security;
using IdentityAuthGuard.Constants;
using IdentityAuthGuard.DTOs;
using IdentityAuthGuard.Services.UserServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PlayerHub.API.Controllers
{
    /// <summary>
    /// Controller for managing user accounts, including creation, login, logout, and token refresh.
    /// </summary>
    [Route("api/account")]
    [ApiController]
    [Produces("application/json")]
    [Authorize(Policy = ApiKeyRequirement.Scheme)]
    public class AccountController(IUserService userService) : AppController
    {
        private readonly IUserService _userService = userService;

        /// <summary>
        /// Creates a new user account.
        /// </summary>
        /// <param name="dto">The user details for account creation.</param>
        /// <returns>An HTTP response indicating the result of the operation.</returns>
        [HttpPost]
        [Authorize(Policy = Schemes.UserScheme, Roles = DefaultRoles.Admin)]
        public async Task<IActionResult> Create([FromBody] UserDTO dto)
        {
            return GenerateResponse(await _userService.CreateAccountAsync(dto));
        }

        /// <summary>
        /// Logs in a user account.
        /// </summary>
        /// <param name="dto">The login details of the user.</param>
        /// <returns>An HTTP response indicating the result of the operation.</returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO dto)
        {
            return GenerateResponse(await _userService.LoginAccountAsync(dto));
        }

        /// <summary>
        /// Logs out the currently authenticated user.
        /// </summary>
        /// <returns>An HTTP response indicating the result of the operation.</returns>
        [HttpPost("logout")]
        [Authorize(Policy = Schemes.UserScheme)]
        public async Task<IActionResult> Logout()
        {
            return GenerateResponse(await _userService.LogoutAsync(IdentityEmail));
        }

        /// <summary>
        /// Refreshes the authentication token for a user.
        /// </summary>
        /// <param name="dto">The token details for refreshing.</param>
        /// <returns>An HTTP response indicating the result of the operation.</returns>
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] TokenDTO dto)
        {
            return GenerateResponse(await _userService.RefreshToken(dto));
        }
    }
}
