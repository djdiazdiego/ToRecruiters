namespace IdentityAuthGuard.Constants
{
    public static class DefaultRoles
    {
        public const string Admin = "admin";
        public const string User = "user";

        public static IReadOnlySet<string> Roles = new HashSet<string>() { Admin, User };
    }
}
