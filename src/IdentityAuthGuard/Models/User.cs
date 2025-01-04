using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace IdentityAuthGuard.Models
{
    public sealed class User : IdentityUser<Guid>
    {
        [Required]
        public override string Email { get; set; } = string.Empty;

        [Required]
        public override string UserName { get; set; } = string.Empty;

        [Required]
        [MinLength(1)]
        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;
        public string? MiddleName { get; set; } = string.Empty;

        [Required]
        [MinLength(1)]
        [MaxLength(100)]
        public string LastName { get; set; } = string.Empty;
        public string? SecondLastName { get; set; } = string.Empty;
        public string? RefreshToken { get; set; } = string.Empty;
        public DateTime RefreshTokenExpiryTime { get; set; }
        public DateTimeOffset CreationDate { get; set; }
        public DateTimeOffset? LastUpdateDate { get; set; }
    }
}
