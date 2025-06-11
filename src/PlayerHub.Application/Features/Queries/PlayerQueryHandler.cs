using Core.Application.CQRS;
using Core.Application.Persistence;
using Core.Wrappers;
using Mapster;
using PlayerHub.Application.DTOs.PlayerDTOs;
using PlayerHub.Application.Pesistence.ReadRepositories;
using PlayerHub.Domain;
using System.Linq.Expressions;

namespace PlayerHub.Application.Features.Queries
{
    public sealed class PlayerQueryHandler(IReadUnitOfWork readUnitOfWork) :
        IFilterQueryHandler<Player, PlayerQuery, PageResponse<PlayerDTO>>
    {
        private readonly IReadUnitOfWork _readUnitOfWork = readUnitOfWork;

        public async Task<PageResponse<PlayerDTO>> Handle(PlayerQuery request, CancellationToken cancellationToken)
        {
            IPlayerReadRepository playerRepository = (IPlayerReadRepository)_readUnitOfWork.GetRepository<Player>();

            static PlayerDTO map(Player item) => item.Adapt<PlayerDTO>();

            return await playerRepository.GetPlayersPageAsync(
                request.OData,
                map,
                BuildSearch,
                cancellationToken);
        }

        public Expression<Func<Player, bool>> BuildSearch(string search) =>
             (x) => x.Name.ToLowerInvariant().Contains(search.ToLowerInvariant());
    }
}
