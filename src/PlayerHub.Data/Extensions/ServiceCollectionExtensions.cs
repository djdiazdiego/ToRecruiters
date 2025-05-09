using Core.Data.Extensions;
using IdentityAuthGuard.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PlayerHub.Data.Contexts;

namespace PlayerHub.Data.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddDataServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContextFactoryServices(configuration);
            services.AddUnitOfWorkServices<ReadDbContext, WriteDbContext>();
        }

        private static void AddDbContextFactoryServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContextFactory<WriteDbContext, WriteDbContextFactory>();
            services.AddDbContextFactory<ReadDbContext, ReadDbContextFactory>();
            services.AddIdentityAuthGuardDbContextFactoryServices(configuration);
        }
    }
}
