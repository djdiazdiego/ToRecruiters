using Core.Application.CQRS;
using Core.Application.Persistence;
using Core.Wrappers;
using PlayerHub.Application.DTOs.PlayerDTOs;
using PlayerHub.Application.Pesistence.ReadRepositories;
using PlayerHub.Domain;

namespace PlayerHub.Application.Features.Queries
{
    public sealed class BestPlayerQueryHandler(IReadUnitOfWork readUnitOfWork) :
        IQueryHandler<BestPlayerQuery, Response<List<BestPlayerDTO>>>
    {
        private readonly IReadUnitOfWork _readUnitOfWork = readUnitOfWork;

        public async Task<Response<List<BestPlayerDTO>>> Handle(BestPlayerQuery request, CancellationToken cancellationToken)
        {
            var playerRepository = (IPlayerReadRepository)_readUnitOfWork.GetRepository<Player>();
            var query = playerRepository.Query;
            List<BestPlayerDTO> players = [];

            foreach (var data in request.Data)
            {
                var position = Enum.Parse<PositionValue>(data.Position);
                var skill = Enum.Parse<SkillValue>(data.Skill);
                var skillName = data.Skill;

                var player = await playerRepository.GetBestPlayerByPositionAndSkillAsync(position, skill, skillName, cancellationToken);

                if (player is not null)
                {
                    players.Add(player);
                }
            }

            return Response<List<BestPlayerDTO>>.Ok(players);
        }
    }
}
