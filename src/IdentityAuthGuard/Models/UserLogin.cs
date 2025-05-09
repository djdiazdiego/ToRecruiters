using Microsoft.AspNetCore.Identity;

namespace IdentityAuthGuard.Models
{
    /// <summary>
    /// Represents a user login associated with a specific user in the identity system.
    /// Inherits from <see cref="IdentityUserLogin{TKey}"/> with a <see cref="Guid"/> as the key type.
    /// </summary>
    public sealed class UserLogin : IdentityUserLogin<Guid>
    {
    }
}
