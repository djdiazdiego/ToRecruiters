using Core.Data.Extensions;
using Core.Data.Interceptors;
using IdentityAuthGuard.Extensions;
using Microsoft.EntityFrameworkCore;
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
            services.AddTransient<ISaveChangesInterceptor, PublishDomainEventsInterceptor>();

            var connection = configuration.GetConnectionString(Constants.CONNECTION_STRING);

            if (string.IsNullOrWhiteSpace(connection))
            {
                throw new InvalidOperationException("The connection string is missing or empty in the configuration.");
            }

            services.AddSingleton<IDbContextFactory<WriteDbContext>>(provider =>
            {
                var interceptors = provider.GetRequiredService<ISaveChangesInterceptor>();

                return new WriteDbContextFactory(configuration, [interceptors]);
            });

            services.AddSingleton<IDbContextFactory<ReadDbContext>>(new ReadDbContextFactory(configuration));

            services.AddIdentityAuthGuardDbContextFactoryServices(configuration);
        }
    }
}
