using Core.Application.Persistence;
using PlayerHub.Domain;

namespace PlayerHub.Application.Pesistence.WriteRepositories
{
    public interface ISkillWriteRepository : IWriteRepository<Skill>
    {
        Task<Skill[]> GetByIdsAsync(SkillValue[] skillValues, CancellationToken cancellationToken);
    }
}
