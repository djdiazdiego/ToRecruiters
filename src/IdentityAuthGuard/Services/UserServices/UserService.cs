using Core.Exceptions;
using Core.Security;
using Core.Wrappers;
using IdentityAuthGuard.Constants;
using IdentityAuthGuard.DTOs;
using IdentityAuthGuard.Extensions;
using IdentityAuthGuard.Models;
using IdentityAuthGuard.Services.GuidGeneratorServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace IdentityAuthGuard.Services.UserServices
{
    /// <summary>
    /// Service for managing user-related operations.
    /// </summary>
    public sealed class UserService(
        UserManager<User> userManager,
        RoleManager<Role> roleManager,
        IGuidGeneratorService guidGenerator,
        IConfiguration configuration) : IUserService
    {
        private readonly UserManager<User> _userManager = userManager;
        private readonly RoleManager<Role> _roleManager = roleManager;
        private readonly IGuidGeneratorService _guidGenerator = guidGenerator;

        private readonly string _issuer = configuration["Jwt:Issuer"] ?? throw new NotFoundException("Jwt issuer not found");
        private readonly string _audience = configuration["Jwt:Audience"] ?? throw new NotFoundException("Jwt audience not found");
        private readonly string _key = configuration["Jwt:Key"] ?? throw new NotFoundException("Jwt key not found");

        /// <inheritdoc />
        public async Task<Response> CreateAccountAsync(UserDTO userDTO)
        {
            if (userDTO is null)
            {
                return Response.Full((int)HttpStatusCode.BadRequest, "Model is empty");
            }

            var user = await _userManager.FindByEmailAsync(userDTO.Email);

            if (user is not null)
            {
                return Response.Full((int)HttpStatusCode.Conflict, "User already registered");
            }

            if (userDTO.Roles is null || userDTO.Roles.Count == 0)
            {
                userDTO.Roles = [DefaultRoles.User];
            }

            var validRoles = new List<string>();

            foreach (var role in userDTO.Roles)
            {
                if (await _roleManager.RoleExistsAsync(role))
                {
                    validRoles.Add(role);
                }
            }

            if (validRoles.Count == 0)
            {
                validRoles.Add(DefaultRoles.User);
            }

            user = new User
            {
                UserName = userDTO.Email,
                Email = userDTO.Email,
                FirstName = userDTO.FirstName,
                LastName = userDTO.LastName,
                MiddleName = userDTO.MiddleName,
                SecondLastName = userDTO.SecondLastName
            };

            var result = await _userManager.CreateAsync(user, userDTO.Password);

            if (!result.Succeeded)
            {
                return result.ErrorResponse("User creation failed due to unknown error.");
            }

            foreach (var role in validRoles)
            {
                await _userManager.AddToRoleAsync(user, role);
            }

            return Response.Ok;
        }

        /// <inheritdoc />
        public async Task<Response> LoginAccountAsync(LoginDTO loginDTO)
        {
            if (loginDTO is null)
            {
                return Response.Full((int)HttpStatusCode.BadRequest, "Model is empty");
            }

            var user = await _userManager.FindByEmailAsync(loginDTO.Email);

            if (user is null)
            {
                return Response.Full((int)HttpStatusCode.NotFound, $"User not found with email: {loginDTO.Email}");
            }

            if (await _userManager.IsLockedOutAsync(user))
            {
                return Response.Full((int)HttpStatusCode.Forbidden, "User is locked out");
            }

            var isCorrectPassword = await _userManager.CheckPasswordAsync(user, loginDTO.Password);

            if (!isCorrectPassword)
            {
                await _userManager.AccessFailedAsync(user);

                if (await _userManager.IsLockedOutAsync(user))
                {
                    return Response.Full((int)HttpStatusCode.Forbidden, "User is locked out due to multiple failed login attempts");
                }

                return Response.Full((int)HttpStatusCode.Unauthorized, "Wrong password");
            }

            await _userManager.ResetAccessFailedCountAsync(user);

            return await GenerateTokenAsync(user, true);
        }

        /// <inheritdoc />
        public async Task<Response> RefreshToken(TokenDTO dto)
        {
            if (dto is null || string.IsNullOrWhiteSpace(dto.AccessToken) || string.IsNullOrWhiteSpace(dto.RefreshToken))
            {
                return Response.Full((int)HttpStatusCode.BadRequest, "Invalid token data");
            }

            var (response, email) = ValidateExpiredToken(dto.AccessToken);

            if (response != null)
            {
                return response;
            }

            var user = await _userManager.FindByEmailAsync(email);

            if (user is null)
            {
                return Response.Full((int)HttpStatusCode.NotFound, "User not found");
            }

            if (string.IsNullOrWhiteSpace(user.RefreshToken) ||
                user.RefreshToken != dto.RefreshToken ||
                user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                return Response.Full((int)HttpStatusCode.Unauthorized, "Invalid or expired refresh token");
            }

            return await GenerateTokenAsync(user, false);
        }

        /// <inheritdoc />
        public async Task<Response> LogoutAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return Response.Full((int)HttpStatusCode.BadRequest, "Email cannot be null or empty");
            }

            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return Response.Full((int)HttpStatusCode.NotFound, $"User not found with email: {email}");
            }

            if (string.IsNullOrWhiteSpace(user.RefreshToken))
            {
                return Response.Full((int)HttpStatusCode.BadRequest, "User is not logged in");
            }

            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddYears(-1);

            var userUpdatedResult = await _userManager.UpdateAsync(user);

            if (!userUpdatedResult.Succeeded)
            {
                return userUpdatedResult.ErrorResponse();
            }

            return Response.Ok;
        }

        /// <summary>
        /// Generates a new JWT token and optionally refresh token expiry time.
        /// </summary>
        /// <param name="user">The user for whom the token is generated.</param>
        /// <param name="populateRefreshTokenExpiryTime">Whether to populate the refresh token expiry time.</param>
        /// <returns>A response containing the generated token.</returns>
        private async Task<Response> GenerateTokenAsync(User user, bool populateRefreshTokenExpiryTime)
        {
            var refreshToken = GenerateRefreshToken();

            user.RefreshToken = refreshToken;

            if (populateRefreshTokenExpiryTime)
            {
                user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(1);
            }

            var userUpdatedResult = await _userManager.UpdateAsync(user);

            if (!userUpdatedResult.Succeeded)
            {
                return userUpdatedResult.ErrorResponse();
            }

            var roles = await _userManager.GetRolesAsync(user);
            var roleClaims = roles?.Select(x => new Claim(ClaimTypes.Role, x)).ToArray() ?? [];
            var userClaims = await _userManager.GetClaimsAsync(user) ?? [];

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, _guidGenerator.New.ToString())
            }
            .Union(roleClaims)
            .Union(userClaims);

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var securityToken = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(30),
                signingCredentials: credentials);

            var handler = new JwtSecurityTokenHandler();

            return Response<TokenDTO>.Ok(new TokenDTO
            {
                AccessToken = handler.WriteToken(securityToken),
                RefreshToken = user.RefreshToken,
                RefreshTokenExpiryTime = user.RefreshTokenExpiryTime
            });
        }

        /// <summary>
        /// Generates a new refresh token.
        /// </summary>
        /// <returns>A new refresh token as a string.</returns>
        private static string GenerateRefreshToken()
        {
            using var generator = RandomNumberGenerator.Create();

            var randomNumber = new byte[64];
            generator.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber)
                .TrimEnd('=') // Remove padding for cleaner token
                .Replace('+', '-') // Replace '+' with '-' for URL safety
                .Replace('/', '_'); // Replace '/' with '_' for URL safety
        }

        /// <summary>
        /// Validates an expired token and extracts the email if valid.
        /// </summary>
        /// <param name="token">The expired token to validate.</param>
        /// <returns>A tuple containing a response and the extracted email.</returns>
        private (Response?, string) ValidateExpiredToken(string token)
        {
            var parameters = Helpers.GetTokenValidationParameters(_issuer, _audience, _key);
            var handler = new JwtSecurityTokenHandler();

            try
            {
                var principal = handler.ValidateToken(token, parameters, out SecurityToken securityToken);

                if (securityToken is not JwtSecurityToken jwtToken ||
                    !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    return (Response.Full((int)HttpStatusCode.Unauthorized, "Invalid token"), string.Empty);
                }

                var email = principal?.FindFirst(ClaimTypes.Email)?.Value;

                if (string.IsNullOrWhiteSpace(email))
                {
                    return (Response.Full((int)HttpStatusCode.Unauthorized, "Invalid token"), string.Empty);
                }

                return (null, email);
            }
            catch (SecurityTokenExpiredException)
            {
                var jwtToken = new JwtSecurityToken(token);

                if (jwtToken.ValidTo < DateTime.UtcNow &&
                    jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    var email = jwtToken.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;

                    if (!string.IsNullOrWhiteSpace(email))
                    {
                        return (null, email);
                    }
                }

                return (Response.Full((int)HttpStatusCode.Unauthorized, "Token expired"), string.Empty);
            }
            catch
            {
                return (Response.Full((int)HttpStatusCode.Unauthorized, "Invalid token"), string.Empty);
            }
        }
    }
}
