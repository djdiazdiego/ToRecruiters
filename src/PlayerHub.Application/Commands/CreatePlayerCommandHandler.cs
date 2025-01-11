using AutoMapper;
using Core.Data.UnitOfWorks;
using Core.Head.CQRS;
using Core.Head.Wrappers;
using Microsoft.EntityFrameworkCore;
using PlayerHub.Application.DTOs.PlayerDTOs;
using PlayerHub.Domain;

namespace PlayerHub.Application.Commands
{
    public sealed class CreatePlayerCommandHandler(IWriteUnitOfWork writeUnitOfWork, IMapper mapper) : ICommandHandler<CreatePlayerCommand, Response<PlayerDTO>>
    {
        private readonly IWriteUnitOfWork _writeUnitOfWork = writeUnitOfWork;
        private readonly IMapper _mapper = mapper;

        public async Task<Response<PlayerDTO>> Handle(CreatePlayerCommand request, CancellationToken cancellationToken)
        {
            var skillRepository = _writeUnitOfWork.GetRepository<Skill>();
            var skillValues = request.Body.Skills.ToArray();

            var skills = await skillRepository.GetQuery().Where(x => skillValues.Contains(x.Id))
                .ToArrayAsync(cancellationToken)
                .ConfigureAwait(false);

            var player = Player.New(request.Body.Name, request.Body.Position, skills);

            var playerRepository = _writeUnitOfWork.GetRepository<Player>();
            playerRepository.Add(player);
            await _writeUnitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            var dto = _mapper.Map<PlayerDTO>(player);

            return Response<PlayerDTO>.Ok(dto);
        }
    }
}
