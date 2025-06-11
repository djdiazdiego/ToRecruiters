using Core.Infrastructure.Seeds;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PlayerHub.Domain;
using PlayerHub.Infrastructure.Contexts;

namespace PlayerHub.Infrastructure.Seeds
{
    public sealed class SkillSeed : ISeed
    {
        public async Task SeedAsync(IServiceProvider provider, CancellationToken cancellationToken = default)
        {
            var dbContextFactory = provider.GetRequiredService<IDbContextFactory<WriteDbContext>>();
            using var context = dbContextFactory.CreateDbContext();
            var query = context.Set<Skill>();
            var updated = false;

            foreach (SkillValue skillValue in Enum.GetValues(typeof(SkillValue)))
            {
                if (!await query.AnyAsync(x => x.Id == skillValue, cancellationToken).ConfigureAwait(false))
                {
                    updated = true;
                    context.Add<Skill>(new(skillValue, skillValue.ToString()));
                }
            }

            if (updated)
            {
                await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            }
        }
    }
}
