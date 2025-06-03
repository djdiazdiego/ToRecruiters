using AutoMapper;
using Core.Application.CQRS;
using Core.Application.Persistence;
using Core.Infrastructure.Helpers;
using Core.Wrappers;
using PlayerHub.Application.DTOs.PlayerDTOs;
using PlayerHub.Domain;
using System.Linq.Expressions;

namespace PlayerHub.Application.Queries
{
    public sealed class PlayerQueryHandler(IReadUnitOfWork readUnitOfWork, IMapper mapper) :
        IFilterQueryHandler<Player, PlayerQuery, PageResponse<PlayerDTO>>
    {
        private readonly IReadUnitOfWork _readUnitOfWork = readUnitOfWork;
        private readonly IMapper _mapper = mapper;

        public async Task<PageResponse<PlayerDTO>> Handle(PlayerQuery request, CancellationToken cancellationToken)
        {
            var playerRepository = _readUnitOfWork.GetRepository<Player>();

            var response = await ODataQueryHelpers.GetPageDataAsync<Player, PlayerDTO>(
                playerRepository.GetQuery(),
                request.OData,
                _mapper,
                BuildSearch,
                cancellationToken);

            return response;
        }

        public Expression<Func<Player, bool>> BuildSearch(string search) =>
             (x) => x.Name.ToLower().Contains(search.ToLower());
    }
}
