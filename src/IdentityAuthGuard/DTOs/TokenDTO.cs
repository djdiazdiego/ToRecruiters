using System.ComponentModel.DataAnnotations;

namespace IdentityAuthGuard.DTOs
{
    public sealed class TokenDTO
    {
        [DataType(DataType.Text)]
        public string AccessToken { get; set; } = string.Empty;

        [DataType(DataType.Text)]
        public string RefreshToken { get; set; } = string.Empty;

        [DataType(DataType.DateTime)]
        public DateTime RefreshTokenExpiryTime { get; set; }
    }
}
