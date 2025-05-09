using Microsoft.AspNetCore.Identity;

namespace IdentityAuthGuard.Models
{
    /// <summary>
    /// Represents the association between a user and a role in the identity system.
    /// This class extends <see cref="IdentityUserRole{TKey}"/> with a <see cref="Guid"/> as the key type.
    /// </summary>
    public sealed class UserRole : IdentityUserRole<Guid>
    {
    }
}
