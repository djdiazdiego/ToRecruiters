using System.ComponentModel.DataAnnotations;

namespace IdentityAuthGuard.DTOs
{
    /// <summary>
    /// Represents a Data Transfer Object (DTO) for tokens used in authentication.
    /// </summary>
    public sealed class TokenDTO
    {
        /// <summary>
        /// Gets or sets the access token used for authentication.
        /// </summary>
        [DataType(DataType.Text)]
        public string AccessToken { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the refresh token used to obtain a new access token.
        /// </summary>
        [DataType(DataType.Text)]
        public string RefreshToken { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the expiration time of the refresh token.
        /// </summary>
        [DataType(DataType.DateTime)]
        public DateTime RefreshTokenExpiryTime { get; set; }
    }
}
