namespace IdentityAuthGuard.Constants
{
    /// <summary>
    /// Provides default roles used for identity and authorization.
    /// </summary>
    public static class DefaultRoles
    {
        /// <summary>
        /// Represents the administrator role.
        /// </summary>
        public const string Admin = "admin";

        /// <summary>
        /// Represents the standard user role.
        /// </summary>
        public const string User = "user";

        /// <summary>
        /// A read-only set containing all default roles.
        /// </summary>
        public static readonly IReadOnlySet<string> Roles = new HashSet<string>() { Admin, User };
    }
}
