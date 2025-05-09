using Microsoft.AspNetCore.Identity;

namespace IdentityAuthGuard.Models
{
    /// <summary>
    /// Represents a claim that is associated with a user.
    /// Inherits from <see cref="IdentityUserClaim{TKey}"/> with a <see cref="Guid"/> as the key type.
    /// </summary>
    public sealed class UserClaim : IdentityUserClaim<Guid>
    {
    }
}
