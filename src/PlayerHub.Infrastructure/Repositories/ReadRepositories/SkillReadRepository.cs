using Core.Infrastructure.Repositories;
using PlayerHub.Application.Pesistence.ReadRepositories;
using PlayerHub.Domain;
using PlayerHub.Infrastructure.Contexts;

namespace PlayerHub.Infrastructure.Repositories.ReadRepositories
{
    public sealed class SkillReadRepository(ReadDbContext context) : ReadRepository<Skill>(context), ISkillReadRepository
    {
        
    }
}
