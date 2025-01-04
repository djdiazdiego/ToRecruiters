using AutoMapper;
using Core.Data.UnitOfWorks;
using Core.Head.CQRS;
using Core.Head.Helpers;
using Core.Head.Wrappers;
using Microsoft.EntityFrameworkCore;
using PlayerHub.Application.DTOs.PlayerDTOs;
using PlayerHub.Domain;
using System.Linq;
using System.Linq.Expressions;

namespace PlayerHub.Application.Queries
{
    public sealed class PlayerQueryHandler(IReadUnitOfWork readUnitOfWork, IMapper mapper) :
        IQueryHandler<PlayerQuery, PageResponse<PlayerDTO>>
    {
        private readonly IReadUnitOfWork _readUnitOfWork = readUnitOfWork;
        private readonly IMapper _mapper = mapper;

        public async Task<PageResponse<PlayerDTO>> Handle(PlayerQuery request, CancellationToken cancellationToken)
        {
            var playerRepository = _readUnitOfWork.GetRepository<Player>();

            // custom search
            Expression<Func<Player, bool>> buildSearch(string search) =>
                (x) => x.Name.ToLower().Contains(search.ToLower());

            var response = await QueryHelpers.GetPageDataAsync<Player, PlayerDTO>(
                playerRepository.GetQuery(),
                request.OData,
                _mapper,
                buildSearch,
                cancellationToken);

            return response;
        }
    }
}
