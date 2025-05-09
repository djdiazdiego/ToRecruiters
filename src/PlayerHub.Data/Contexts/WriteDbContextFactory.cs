using Core.Data;
using Core.Data.Factories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace PlayerHub.Data.Contexts
{
    public sealed class WriteDbContextFactory :
        DbContextFactory<WriteDbContext>,
        IDesignTimeDbContextFactory<WriteDbContext>,
        IDbContextFactory<WriteDbContext>
    {


        public WriteDbContextFactory() :
            base(Constants.CONNECTION_STRING, Constants.MIGRATIONS_ASSEMBLY, DbTypes.SqlServer)
        { }

        public WriteDbContextFactory(IConfiguration configuration) :
            base(configuration, Constants.CONNECTION_STRING, Constants.MIGRATIONS_ASSEMBLY, DbTypes.SqlServer)
        { }
    }
}
