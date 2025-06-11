using Core.Application.CQRS;
using Core.Application.Persistence;
using Core.Wrappers;
using Mapster;
using PlayerHub.Application.DTOs.PlayerDTOs;
using PlayerHub.Application.Pesistence.WriteRepositories;
using PlayerHub.Domain;

namespace PlayerHub.Application.Features.Commands
{
    public sealed class UpdatePlayerCommandHandler(IWriteUnitOfWork writeUnitOfWork) :
        ICommandHandler<UpdatePlayerCommand, Response<PlayerDTO>>
    {
        private readonly IWriteUnitOfWork _writeUnitOfWork = writeUnitOfWork;

        public async Task<Response<PlayerDTO>> Handle(UpdatePlayerCommand request, CancellationToken cancellationToken)
        {
            var playerRepository = (IPlayerWriteRepository)_writeUnitOfWork.GetRepository<Player>();

            var player = await playerRepository.GetByIdWithSkillsAsync(request.Body.Id, cancellationToken);

            var skillRepository = (ISkillWriteRepository)_writeUnitOfWork.GetRepository<Skill>();
            var skillValues = request.Body.Skills.ToArray();

            var skills = await skillRepository.GetByIdsAsync(skillValues, cancellationToken);

            player.Update(request.Body.Name, request.Body.Position, skills);

            playerRepository.Update(player);
            await _writeUnitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            var dto = player.Adapt<PlayerDTO>();

            return Response<PlayerDTO>.Ok(dto);
        }
    }
}
