using Core.Infrastructure;
using Core.Infrastructure.Factories;
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
            base(Constants.CONNECTION_STRING, Constants.MIGRATIONS_ASSEMBLY, DbTypes.SqlServer)
        { }

        public ReadDbContextFactory(IConfiguration configuration) :
            base(configuration, Constants.CONNECTION_STRING, Constants.MIGRATIONS_ASSEMBLY, DbTypes.SqlServer)
        { }
    }
}
