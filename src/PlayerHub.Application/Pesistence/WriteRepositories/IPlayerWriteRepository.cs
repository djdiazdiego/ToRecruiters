using Core.Application.Persistence;
using PlayerHub.Domain;

namespace PlayerHub.Application.Pesistence.WriteRepositories
{
    public interface IPlayerWriteRepository : IWriteRepository<Player>
    {
        Task<Player> GetByIdWithSkillsAsync(Guid id, CancellationToken cancellationToken);
    }
}
