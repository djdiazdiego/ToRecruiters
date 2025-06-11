using Core.Application.Persistence;
using Core.Wrappers;
using Microsoft.AspNetCore.OData.Query;
using PlayerHub.Application.DTOs.PlayerDTOs;
using PlayerHub.Domain;
using System.Linq.Expressions;

namespace PlayerHub.Application.Pesistence.ReadRepositories
{
    public interface IPlayerReadRepository : IReadRepository<Player>
    {
        Task<PageResponse<TResponse>> GetPlayersPageAsync<TResponse>(
            ODataQueryOptions<Player> oDataOptions,
            Func<Player, TResponse> map,
            Func<string, Expression<Func<Player, bool>>>? searchPredicateBuilder = null,
            CancellationToken cancellationToken = default) where TResponse : class;

        Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken);

        Task<BestPlayerDTO?> GetBestPlayerByPositionAndSkillAsync(
            PositionValue position,
            SkillValue skill,
            string skillName,
            CancellationToken cancellationToken);
    }
}
