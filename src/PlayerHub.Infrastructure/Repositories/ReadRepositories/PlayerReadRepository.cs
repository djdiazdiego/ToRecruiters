using Core.Infrastructure.Extensions;
using Core.Infrastructure.Repositories;
using Core.Wrappers;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;
using PlayerHub.Application.DTOs;
using PlayerHub.Application.DTOs.PlayerDTOs;
using PlayerHub.Application.Pesistence.ReadRepositories;
using PlayerHub.Domain;
using PlayerHub.Infrastructure.Contexts;
using System.Linq.Expressions;

namespace PlayerHub.Infrastructure.Repositories.ReadRepositories
{
    public sealed class PlayerReadRepository(ReadDbContext context) : ReadRepository<Player>(context), IPlayerReadRepository
    {
        public Task<PageResponse<TResponse>> GetPlayersPageAsync<TResponse>(
            ODataQueryOptions<Player> oDataOptions,
            Func<Player, TResponse> map,
            Func<string, Expression<Func<Player, bool>>>? searchPredicateBuilder = null,
            CancellationToken cancellationToken = default) where TResponse : class =>
            Query.GetPageDataAsync(
                oDataOptions,
                map,
                searchPredicateBuilder,
                cancellationToken);

        public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken) =>
            await Query.AnyAsync(x => x.Id == id, cancellationToken).ConfigureAwait(false);

        public async Task<BestPlayerDTO?> GetBestPlayerByPositionAndSkillAsync(
            PositionValue position,
            SkillValue skill,
            string skillName,
            CancellationToken cancellationToken)
        {
            return await Query.Include(x => x.Skills)
                .Where(x => x.Position == position && x.Skills.Any(y => y.Id == skill))
                .Select(x => new BestPlayerDTO
                {
                    Name = x.Name,
                    Position = x.Position.ToString(),
                    Skill = new SkillDTO
                    {
                        Name = skillName,
                        Value = skill.GetHashCode()
                    },
                    CreationDate = x.CreationDate,
                    LastUpdateDate = x.LastUpdateDate
                })
                .FirstOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);
        }
    }
}
