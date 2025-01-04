using Core.Data;
using Core.Data.Factories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace PlayerHub.Data.Contexts
{
    public sealed class ReadDbContextFactory :
        DbContextFactory<ReadDbContext>,
        IDesignTimeDbContextFactory<ReadDbContext>,
        IDbContextFactory<ReadDbContext>
    {
        public ReadDbContextFactory() :
            base(Constants.MIGRATIONS_ASSEMBLY, DbTypes.SqlServer)
        { }

        public ReadDbContextFactory(IConfiguration configuration) :
            base(Constants.MIGRATIONS_ASSEMBLY, DbTypes.SqlServer, configuration)
        { }
    }
}
