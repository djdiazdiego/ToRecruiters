using Core.Data.Extensions;
using Core.Data.Interceptors;
using IdentityAuthGuard.Extensions;
using Microsoft.EntityFrameworkCore.Diagnostics;
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
            services.AddScoped<ISaveChangesInterceptor, PublishDomainEventsInterceptor>();

            services.AddDbContextFactory<WriteDbContext, WriteDbContextFactory>((provider, options) =>
            {
                options.AddInterceptors(provider.GetServices<ISaveChangesInterceptor>());
            });
            services.AddDbContextFactory<ReadDbContext, ReadDbContextFactory>();
            services.AddIdentityAuthGuardDbContextFactoryServices(configuration);
        }
    }
}
