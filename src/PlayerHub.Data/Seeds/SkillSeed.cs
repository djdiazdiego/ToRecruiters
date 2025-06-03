using Core.Application.Persistence;
using Core.Infrastructure.Seeds;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PlayerHub.Domain;

namespace PlayerHub.Data.Seeds
{
    public sealed class SkillSeed : ISeed
    {
        public async Task SeedAsync(IServiceProvider provider, CancellationToken cancellationToken = default)
        {
            using var scope = provider.CreateScope();

            var unitOfWork = scope.ServiceProvider.GetRequiredService<IWriteUnitOfWork>();
            var skillRepository = unitOfWork.GetRepository<Skill>();
            var updated = false;

            foreach (SkillValue skillValue in Enum.GetValues(typeof(SkillValue)))
            {
                if (!(await skillRepository.GetQuery().AnyAsync(x => x.Id == skillValue, cancellationToken).ConfigureAwait(false)))
                {
                    updated = true;
                    skillRepository.Add(new(skillValue, skillValue.ToString()));
                }
            }

            if (updated)
            {
                await unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            }
        }
    }
}
