using Core.Exceptions;
using Security;
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
                throw new BadRequestException("Model is empty");
            }

            var user = await _userManager.FindByEmailAsync(userDTO.Email);

            if (user is not null)
            {
                throw new AlreadyExistsException($"User with email {userDTO.Email} already exists.");
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

            IdentityResultExtensions.ErrorResponse(result, "User creation failed.");

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
                throw new BadRequestException("Model is empty");
            }

            var user = await _userManager.FindByEmailAsync(loginDTO.Email) ??
                throw new NotFoundException($"User with email {loginDTO.Email} not found.");

            if (await _userManager.IsLockedOutAsync(user))
            {
                throw new ForbiddenException("User is locked out.");
            }

            var isCorrectPassword = await _userManager.CheckPasswordAsync(user, loginDTO.Password);

            if (!isCorrectPassword)
            {
                IdentityResultExtensions.ErrorResponse(await _userManager.AccessFailedAsync(user));
                IdentityResultExtensions.ErrorResponse(await _userManager.UpdateSecurityStampAsync(user));

                if (await _userManager.IsLockedOutAsync(user))
                {
                    throw new ForbiddenException("User is locked out due to multiple failed login attempts.");
                }

                throw new UnauthorizedAccessException("Wrong password.");
            }

            IdentityResultExtensions.ErrorResponse(await _userManager.ResetAccessFailedCountAsync(user));

            return await GenerateTokenAsync(user, true);
        }

        /// <inheritdoc />
        public async Task<Response> RefreshToken(TokenDTO dto)
        {
            if (dto is null || string.IsNullOrWhiteSpace(dto.AccessToken) || string.IsNullOrWhiteSpace(dto.RefreshToken))
            {
                throw new BadRequestException(dto is null ? "Model is empty or invalid token data" : "Invalid token data");
            }

            var email = ValidateExpiredToken(dto.AccessToken);

            var user = await _userManager.FindByEmailAsync(email) ?? 
                throw new NotFoundException($"User with email {email} not found.");

            if (string.IsNullOrWhiteSpace(user.RefreshToken) ||
                user.RefreshToken != dto.RefreshToken ||
                user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                IdentityResultExtensions.ErrorResponse(await _userManager.UpdateSecurityStampAsync(user));
                throw new UnauthorizedAccessException("Invalid or expired refresh token.");
            }

            return await GenerateTokenAsync(user, false);
        }

        /// <inheritdoc />
        public async Task<Response> LogoutAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new BadRequestException("Email cannot be null or empty");
            }

            var user = await _userManager.FindByEmailAsync(email) ??
                throw new NotFoundException($"User not found with email: {email}");

            if (string.IsNullOrWhiteSpace(user.RefreshToken))
            {
                throw new UnauthorizedAccessException("User is not logged in.");
            }

            IdentityResultExtensions.ErrorResponse(await _userManager.UpdateSecurityStampAsync(user));

            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddYears(-1);

            IdentityResultExtensions.ErrorResponse(await _userManager.UpdateAsync(user));

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

            IdentityResultExtensions.ErrorResponse(await _userManager.UpdateAsync(user));

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
        private string ValidateExpiredToken(string token)
        {
            var parameters = Helpers.GetTokenValidationParameters(_issuer, _audience, _key);
            var handler = new JwtSecurityTokenHandler();

            try
            {
                var principal = handler.ValidateToken(token, parameters, out SecurityToken securityToken);

                if (securityToken is not JwtSecurityToken jwtToken ||
                    !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    throw new UnauthorizedAccessException("Invalid token signature.");
                }

                var email = principal?.FindFirst(ClaimTypes.Email)?.Value;

                if (string.IsNullOrWhiteSpace(email))
                {
                    throw new UnauthorizedAccessException("Email claim not found in token.");
                }

                return email;
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
                        return email;
                    }
                }

                throw new UnauthorizedAccessException("Token expired or email claim not found.");
            }
            catch
            {
                throw new UnauthorizedAccessException("Invalid token.");
            }
        }
    }
}
