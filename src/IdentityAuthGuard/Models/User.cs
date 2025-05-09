using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace IdentityAuthGuard.Models
{
    /// <summary>
    /// Represents a user entity that extends the IdentityUser class with additional properties.
    /// </summary>
    public sealed class User : IdentityUser<Guid>
    {
        /// <summary>
        /// Gets or sets the email address of the user. This property is required.
        /// </summary>
        [Required]
        public override string? Email { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the username of the user. This property is required.
        /// </summary>
        [Required]
        public override string? UserName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the first name of the user. This property is required and has a maximum length of 100 characters.
        /// </summary>
        [Required]
        [MinLength(1)]
        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the middle name of the user. This property is optional.
        /// </summary>
        public string? MiddleName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the last name of the user. This property is required and has a maximum length of 100 characters.
        /// </summary>
        [Required]
        [MinLength(1)]
        [MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the second last name of the user. This property is optional.
        /// </summary>
        public string? SecondLastName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the refresh token for the user. This property is optional.
        /// </summary>
        public string? RefreshToken { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the expiry time of the refresh token.
        /// </summary>
        public DateTime RefreshTokenExpiryTime { get; set; }

        /// <summary>
        /// Gets or sets the creation date of the user record.
        /// </summary>
        public DateTimeOffset CreationDate { get; set; }

        /// <summary>
        /// Gets or sets the last update date of the user record. This property is optional.
        /// </summary>
        public DateTimeOffset? LastUpdateDate { get; set; }
    }
}
