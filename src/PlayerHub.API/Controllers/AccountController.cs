using IdentityAuthGuard.Constants;
using IdentityAuthGuard.Contracts;
using IdentityAuthGuard.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlayerHub.API.Filters;

namespace PlayerHub.API.Controllers
{
    [Route("api/account")]
    [ApiController]
    [Produces("application/json")]
    [Authorize(Policy = ApiKeyRequirement.Scheme)]
    public class AccountController(IUserService userService) : AppController
    {
        private readonly IUserService _userService = userService;

        /// <summary>
        /// Create user
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Policy = Schemes.UserScheme, Roles = DefaultRoles.Admin)]
        public async Task<IActionResult> Create([FromBody] UserDTO dto)
        {
            return GenerateResponse(await _userService.CreateAccountAsync(dto));
        }

        /// <summary>
        /// Login
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO dto)
        {
            return GenerateResponse(await _userService.LoginAccountAsync(dto));
        }

        /// <summary>
        /// Logout
        /// </summary>
        /// <returns></returns>
        [HttpPost("logout")]
        [Authorize(Policy = Schemes.UserScheme)]
        public async Task<IActionResult> Logout()
        {
            return GenerateResponse(await _userService.LogoutAsync(IdentityEmail));
        }

        /// <summary>
        /// Refresh token
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] TokenDTO dto)
        {
            return GenerateResponse(await _userService.RefreshToken(dto));
        }
    }
}
