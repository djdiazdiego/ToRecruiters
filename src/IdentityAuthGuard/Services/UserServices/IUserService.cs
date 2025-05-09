using Core.Wrappers;
using IdentityAuthGuard.DTOs;

namespace IdentityAuthGuard.Services.UserServices
{
    /// <summary>
    /// Interface for user-related operations.
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// Creates a new user account.
        /// </summary>
        /// <param name="userDTO">The user details for account creation.</param>
        /// <returns>A response indicating the result of the operation.</returns>
        Task<Response> CreateAccountAsync(UserDTO userDTO);

        /// <summary>
        /// Logs in a user account.
        /// </summary>
        /// <param name="loginDTO">The login details of the user.</param>
        /// <returns>A response indicating the result of the operation.</returns>
        Task<Response> LoginAccountAsync(LoginDTO loginDTO);

        /// <summary>
        /// Logs out a user account.
        /// </summary>
        /// <param name="email">The email of the user to log out.</param>
        /// <returns>A response indicating the result of the operation.</returns>
        Task<Response> LogoutAsync(string email);

        /// <summary>
        /// Refreshes the authentication token.
        /// </summary>
        /// <param name="dto">The token details for refreshing.</param>
        /// <returns>A response indicating the result of the operation.</returns>
        Task<Response> RefreshToken(TokenDTO dto);
    }
}
