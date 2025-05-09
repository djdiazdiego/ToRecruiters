using Microsoft.AspNetCore.Identity;

namespace IdentityAuthGuard.Models
{
    /// <summary>
    /// Represents a role in the identity system, inheriting from <see cref="IdentityRole{Guid}"/>.
    /// </summary>
    public sealed class Role : IdentityRole<Guid>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Role"/> class with the specified role name.
        /// </summary>
        /// <param name="role">The name of the role.</param>
        public Role(string role) : base(role) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Role"/> class.
        /// </summary>
        public Role() : base() { }
    }
}
