using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace IdentityAuthGuard
{
    public static class Helpers
    {
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
