using Core.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using PlayerHub.Application.Pesistence.WriteRepositories;
using PlayerHub.Domain;
using PlayerHub.Infrastructure.Contexts;

namespace PlayerHub.Infrastructure.Repositories.WriteRepositories
{
    public sealed class PlayerWriteRepository(WriteDbContext context) : WriteRepository<Player>(context), IPlayerWriteRepository
    {
        public async Task<Player> GetByIdWithSkillsAsync(Guid id, CancellationToken cancellationToken)
        {
            return await Query
                .Include(x => x.Skills)
                .Where(x => x.Id == id)
                .FirstAsync(cancellationToken)
                .ConfigureAwait(false);
        }
    }
}
