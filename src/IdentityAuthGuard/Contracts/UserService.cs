using Core.Head.Exceptions;
using Core.Head.Wrappers;
using IdentityAuthGuard.Constants;
using IdentityAuthGuard.DTOs;
using IdentityAuthGuard.Extensions;
using IdentityAuthGuard.Models;
using IdentityAuthGuard.Services.GuidGeneratorService;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace IdentityAuthGuard.Contracts
{
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

            var roles = new List<string>();

            foreach (var role in userDTO.Roles)
            {
                if (await _roleManager.RoleExistsAsync(role))
                {
                    roles.Add(role);
                }
            }

            if (roles.Count == 0)
            {
                roles.Add(DefaultRoles.User);
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
                return result.ErrorResponse();
            }

            foreach (var role in userDTO.Roles)
            {
                await _userManager.AddToRoleAsync(user, role);
            }

            return Response.Ok;
        }

        public async Task<Response> LoginAccountAsync(LoginDTO loginDTO)
        {
            if (loginDTO is null)
            {
                return Response.Full((int)HttpStatusCode.BadRequest, "Model is empty");
            }

            var user = await _userManager.FindByEmailAsync(loginDTO.Email);

            if (user == null)
            {
                return Response.Full((int)HttpStatusCode.NotFound, $"User not found with email: {loginDTO.Email}");
            }

            if (await _userManager.IsLockedOutAsync(user))
            {
                return Response.Full((int)HttpStatusCode.Forbidden, "User is locked out");
            }

            var isCorrectPasword = await _userManager.CheckPasswordAsync(user, loginDTO.Password);

            if (!isCorrectPasword)
            {
                return Response.Full((int)HttpStatusCode.Unauthorized, "Wrong password");
            }

            return await GenerateTokenAsync(user, true);
        }

        public async Task<Response> RefreshToken(TokenDTO dto)
        {
            var (response, email) = ValidateExpiredToken(dto.AccessToken);

            if (response != null)
            {
                return response;
            }

            var user = await _userManager.FindByEmailAsync(email);

            if (user is null || user.RefreshToken is null ||
                user.RefreshToken != dto.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                return Response.Full((int)HttpStatusCode.Unauthorized, "Invalid refresh token");
            }

            return await GenerateTokenAsync(user, false);
        }

        public async Task<Response> LogoutAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return Response.Full((int)HttpStatusCode.NotFound, $"User not found with email: {email}");
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

        private static string GenerateRefreshToken()
        {
            using var generator = RandomNumberGenerator.Create();

            var randomNumber = new byte[32];
            generator.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        private (Response?, string) ValidateExpiredToken(string token)
        {
            var parameters = Helpers.GetTokenValidationParameters(_issuer, _audience, _key);

            var handler = new JwtSecurityTokenHandler();

            try
            {
                var principal = handler.ValidateToken(token, parameters, out SecurityToken securityToken);

                var email = principal?.FindFirst(x => x.Type == ClaimTypes.Email)?.Value;

                if (string.IsNullOrEmpty(email))
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
                    var email = jwtToken.Claims.Where(x => x.Type == ClaimTypes.Email)
                        .Select(x => x.Value)
                        .First();

                    return (null, email);
                }

                return (Response.Full((int)HttpStatusCode.Unauthorized, "Invalid token"), string.Empty);
            }
            catch
            {
                return (Response.Full((int)HttpStatusCode.Unauthorized, "Invalid token"), string.Empty);
            }
        }
    }
}
