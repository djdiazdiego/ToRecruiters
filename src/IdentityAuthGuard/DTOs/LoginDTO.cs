using System.ComponentModel.DataAnnotations;

namespace IdentityAuthGuard.DTOs
{
    /// <summary>
    /// Represents the data transfer object for user login.
    /// </summary>
    public sealed class LoginDTO
    {
        /// <summary>
        /// Gets or sets the email address of the user.
        /// </summary>
        /// <remarks>
        /// This field is required and must be a valid email address.
        /// </remarks>
        [Required]
        [EmailAddress]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the password of the user.
        /// </summary>
        /// <remarks>
        /// This field is required and must be a valid password.
        /// </remarks>
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }
}
