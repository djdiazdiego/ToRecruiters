using Microsoft.AspNetCore.Identity;

namespace IdentityAuthGuard.Models
{
    /// <summary>
    /// Represents a claim that is associated with a specific role.
    /// Inherits from <see cref="IdentityRoleClaim{TKey}"/> with a <see cref="Guid"/> as the key type.
    /// </summary>
    public sealed class RoleClaim : IdentityRoleClaim<Guid>
    {
    }
}
