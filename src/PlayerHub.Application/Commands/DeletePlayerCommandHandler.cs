using Core.Application.CQRS;
using Core.Application.Persistence;
using Core.Wrappers;
using Microsoft.EntityFrameworkCore;
using PlayerHub.Domain;

namespace PlayerHub.Application.Commands
{
    public sealed class DeletePlayerCommandHandler(IWriteUnitOfWork writeUnitOfWork) :
        ICommandHandler<DeletePlayerCommand, Response>
    {
        private readonly IWriteUnitOfWork _writeUnitOfWork = writeUnitOfWork;

        public async Task<Response> Handle(DeletePlayerCommand request, CancellationToken cancellationToken)
        {
            var playerRepository = _writeUnitOfWork.GetRepository<Player>();

            var player = await playerRepository.GetQuery()
                .Where(x => x.Id == request.Id)
                .FirstAsync(cancellationToken)
                .ConfigureAwait(false);

            playerRepository.Remove(player);
            await _writeUnitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            return Response.Ok;
        }
    }
}
