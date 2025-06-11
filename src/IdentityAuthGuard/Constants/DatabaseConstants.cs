using Core.Infrastructure.Common;

namespace IdentityAuthGuard.Constants
{
    /// <summary>
    /// Contains constant values used throughout the IdentityAuthGuard application context.
    /// </summary>
    public static class DatabaseConstants
    {
        /// <summary>
        /// The name of the assembly that contains the database migrations.
        /// </summary>
        public const string MIGRATIONS_ASSEMBLY = "IdentityAuthGuard";

        /// <summary>
        /// The name of the connection string used to connect to the application's database.
        /// </summary>
        public const string CONNECTION_STRING = "IdentityAuthGuardDbConnection";

        /// <summary>
        /// The default schema used for the database objects in the application.
        /// </summary>
        public const string DEFAULT_SCHEMA = "dbo.IdentityAuthGuard";

        /// <summary>
        /// The type of database used by the application.
        /// </summary>
        public const DbTypes DB_TYPE = DbTypes.SqlServer;
    }
}
