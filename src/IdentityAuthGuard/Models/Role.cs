using Microsoft.AspNetCore.Identity;

namespace IdentityAuthGuard.Models
{
    public sealed class Role : IdentityRole<Guid>
    {
        public Role(string role) : base(role) { }
        public Role() : base() { }
    }
}
