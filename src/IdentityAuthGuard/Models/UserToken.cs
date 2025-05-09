using Microsoft.AspNetCore.Identity;

namespace IdentityAuthGuard.Models
{
    /// <summary>
    /// Represents a token associated with a user for authentication purposes.
    /// Inherits from <see cref="IdentityUserToken{TKey}"/> with a <see cref="Guid"/> as the key type.
    /// </summary>
    public sealed class UserToken : IdentityUserToken<Guid>
    {
    }
}
