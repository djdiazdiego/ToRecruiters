using System.ComponentModel.DataAnnotations;

namespace IdentityAuthGuard.DTOs
{
    /// <summary>
    /// Represents a Data Transfer Object (DTO) for user information.
    /// </summary>
    public sealed class UserDTO
    {
        /// <summary>
        /// Gets or sets the first name of the user.
        /// </summary>
        [Required]
        [MinLength(1)]
        [MaxLength(100)]
        [DataType(DataType.Text)]
        public string FirstName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the middle name of the user. This field is optional.
        /// </summary>
        [DataType(DataType.Text)]
        public string? MiddleName { get; set; }

        /// <summary>
        /// Gets or sets the last name of the user.
        /// </summary>
        [Required]
        [MinLength(1)]
        [MaxLength(100)]
        [DataType(DataType.Text)]
        public string LastName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the second last name of the user. This field is optional.
        /// </summary>
        [DataType(DataType.Text)]
        public string? SecondLastName { get; set; }

        /// <summary>
        /// Gets or sets the email address of the user.
        /// </summary>
        [Required]
        [EmailAddress]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the password of the user.
        /// </summary>
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the confirmation password of the user. Must match the <see cref="Password"/>.
        /// </summary>
        [Required]
        [DataType(DataType.Password)]
        [Compare(nameof(Password))]
        public string ConfirmPassword { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the list of roles assigned to the user.
        /// </summary>
        public List<string> Roles { get; set; } = [];
    }
}
