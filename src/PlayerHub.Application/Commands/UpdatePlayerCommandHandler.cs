using AutoMapper;
using Core.Data.UnitOfWorks;
using Core.Head.CQRS;
using Core.Head.Wrappers;
using Microsoft.EntityFrameworkCore;
using PlayerHub.Application.DTOs.PlayerDTOs;
using PlayerHub.Domain;

namespace PlayerHub.Application.Commands
{
    public sealed class UpdatePlayerCommandHandler(IWriteUnitOfWork writeUnitOfWork, IMapper mapper) :
        ICommandHandler<UpdatePlayerCommand, Response<PlayerDTO>>
    {
        private readonly IWriteUnitOfWork _writeUnitOfWork = writeUnitOfWork;
        private readonly IMapper _mapper = mapper;

        public async Task<Response<PlayerDTO>> Handle(UpdatePlayerCommand request, CancellationToken cancellationToken)
        {
            var playerRepository = _writeUnitOfWork.GetRepository<Player>();

            var player = await playerRepository.GetQuery()
                .Include(x => x.Skills)
                .Where(x => x.Id == request.Body.Id)
                .FirstAsync(cancellationToken)
                .ConfigureAwait(false);

            var skillRepository = _writeUnitOfWork.GetRepository<Skill>();
            var skillValues = request.Body.Skills.ToArray();

            var skills = await skillRepository.GetQuery()
                .Where(x => skillValues.Contains(x.Id))
                .ToArrayAsync(cancellationToken)
                .ConfigureAwait(false);

            player.Update(request.Body.Name, request.Body.Position, skills);

            playerRepository.Update(player);
            await _writeUnitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            var dto = _mapper.Map<PlayerDTO>(player);

            return Response<PlayerDTO>.Ok(dto);
        }
    }
}
