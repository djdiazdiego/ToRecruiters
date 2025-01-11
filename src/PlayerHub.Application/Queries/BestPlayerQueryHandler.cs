using Core.Data.UnitOfWorks;
using Core.Head.CQRS;
using Core.Head.Wrappers;
using Microsoft.EntityFrameworkCore;
using PlayerHub.Application.DTOs.PlayerDTOs;
using PlayerHub.Domain;

namespace PlayerHub.Application.Queries
{
    public sealed class BestPlayerQueryHandler(IReadUnitOfWork readUnitOfWork) :
        IQueryHandler<BestPlayerQuery, Response<List<BestPlayerDTO>>>
    {
        private readonly IReadUnitOfWork _readUnitOfWork = readUnitOfWork;

        public async Task<Response<List<BestPlayerDTO>>> Handle(BestPlayerQuery request, CancellationToken cancellationToken)
        {
            var playerRepository = _readUnitOfWork.GetRepository<Player>();
            var query = playerRepository.GetQuery();
            List<BestPlayerDTO> players = [];

            foreach (var data in request.Data)
            {
                var position = Enum.Parse<PositionValue>(data.Position);
                var skill = Enum.Parse<SkillValue>(data.Skill);
                var player = await query.Include(x => x.Skills)
                    .Where(x => x.Position == position && x.Skills.Any(y => y.Id == skill))
                    .Select(x => new BestPlayerDTO
                    {
                        Name = x.Name,
                        Position = x.Position.ToString(),
                        Skill = new DTOs.SkillDTO
                        {
                            Name = data.Skill,
                            Value = skill.GetHashCode()
                        },
                        CreationDate = x.CreationDate,
                        LastUpdateDate = x.LastUpdateDate
                    })
                    .FirstOrDefaultAsync(cancellationToken)
                    .ConfigureAwait(false);

                if(player is not null)
                {
                    players.Add(player);
                }
            }

            return Response<List<BestPlayerDTO>>.Ok(players);
        }
    }
}
