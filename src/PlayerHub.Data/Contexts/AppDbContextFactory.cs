using Core.Data;
using Core.Data.Factories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace PlayerHub.Data.Contexts
{
    public sealed class AppDbContextFactory :
        DbContextFactory<AppDbContext>,
        IDesignTimeDbContextFactory<AppDbContext>,
        IDbContextFactory<AppDbContext>
    {
        public AppDbContextFactory() : base(Constants.MIGRATIONS_ASSEMBLY, DbTypes.SqlServer) { }

        public AppDbContextFactory(IConfiguration configuration) : base(Constants.MIGRATIONS_ASSEMBLY, DbTypes.SqlServer, configuration) { }
    }
}
