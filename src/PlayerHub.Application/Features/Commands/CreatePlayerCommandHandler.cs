using Core.Application.CQRS;
using Core.Application.Persistence;
using Core.Wrappers;
using Mapster;
using PlayerHub.Application.DTOs.PlayerDTOs;
using PlayerHub.Application.Pesistence.WriteRepositories;
using PlayerHub.Domain;

namespace PlayerHub.Application.Features.Commands
{
    public sealed class CreatePlayerCommandHandler(IWriteUnitOfWork writeUnitOfWork) : ICommandHandler<CreatePlayerCommand, Response<PlayerDTO>>
    {
        private readonly IWriteUnitOfWork _writeUnitOfWork = writeUnitOfWork;

        public async Task<Response<PlayerDTO>> Handle(CreatePlayerCommand request, CancellationToken cancellationToken)
        {
            var skillRepository = (ISkillWriteRepository)_writeUnitOfWork.GetRepository<Skill>();
            var skillValues = request.Body.Skills.ToArray();

            var skills = await skillRepository.GetByIdsAsync(skillValues, cancellationToken);
            var player = Player.New(request.Body.Name, request.Body.Position, skills);

            var playerRepository = _writeUnitOfWork.GetRepository<Player>();
            playerRepository.Add(player);

            await _writeUnitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            var dto = player.Adapt<PlayerDTO>();

            return Response<PlayerDTO>.Ok(dto);
        }
    }
}
