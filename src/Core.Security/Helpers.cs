using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Core.Security
{
    /// <summary>
    /// Provides helper methods for token validation.
    /// </summary>
    public static class Helpers
    {
        /// <summary>
        /// Creates and returns a <see cref="TokenValidationParameters"/> object configured with the specified parameters.
        /// </summary>
        /// <param name="issuer">The expected issuer of the token.</param>
        /// <param name="audience">The expected audience of the token.</param>
        /// <param name="key">The secret key used to sign the token.</param>
        /// <returns>A configured <see cref="TokenValidationParameters"/> object.</returns>
        public static TokenValidationParameters GetTokenValidationParameters(string issuer, string audience, string key)
        {
            return new TokenValidationParameters
            {
                ClockSkew = TimeSpan.Zero,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,
                ValidIssuer = issuer,
                ValidAudience = audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
            };
        }
    }
}
