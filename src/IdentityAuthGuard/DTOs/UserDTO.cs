using System.ComponentModel.DataAnnotations;

namespace IdentityAuthGuard.DTOs
{
    public sealed class UserDTO
    {
        [Required]
        [MinLength(1)]
        [MaxLength(100)]
        [DataType(DataType.Text)]
        public string FirstName { get; set; } = string.Empty;

        [DataType(DataType.Text)]
        public string? MiddleName { get; set; }

        [Required]
        [MinLength(1)]
        [MaxLength(100)]
        [DataType(DataType.Text)]
        public string LastName { get; set; } = string.Empty;

        [DataType(DataType.Text)]
        public string? SecondLastName { get; set; }

        [Required]
        [EmailAddress]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [Compare(nameof(Password))]
        public string ConfirmPassword { get; set; } = string.Empty;

        public List<string> Roles { get; set; } = [];
    }
}
