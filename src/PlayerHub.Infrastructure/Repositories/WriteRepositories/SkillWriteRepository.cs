using Core.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using PlayerHub.Application.Pesistence.WriteRepositories;
using PlayerHub.Domain;
using PlayerHub.Infrastructure.Contexts;

namespace PlayerHub.Infrastructure.Repositories.WriteRepositories
{
    public sealed class SkillWriteRepository(WriteDbContext context) : WriteRepository<Skill>(context), ISkillWriteRepository
    {
        public async Task<Skill[]> GetByIdsAsync(SkillValue[] skillValues, CancellationToken cancellationToken)
        {
            return await Query.Where(x => skillValues.Contains(x.Id))
                .ToArrayAsync(cancellationToken)
                .ConfigureAwait(false);
        }
    }
}
