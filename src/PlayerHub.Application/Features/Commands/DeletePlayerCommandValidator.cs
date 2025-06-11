using Core.Application.Persistence;
using FluentValidation;
using PlayerHub.Application.Pesistence.ReadRepositories;
using PlayerHub.Domain;

namespace PlayerHub.Application.Features.Commands
{
    public sealed class DeletePlayerCommandValidator : AbstractValidator<DeletePlayerCommand>
    {
        public DeletePlayerCommandValidator(IReadUnitOfWork readUnitOfWork)
        {
            RuleFor(x => x.Id).NotEqual(default(Guid)).WithMessage("Invalid user id")
                .MustAsync((x, id, ct) =>
                {
                    var repository = (IPlayerReadRepository)readUnitOfWork.GetRepository<Player>();
                    return repository.ExistsAsync(id, ct);
                })
                .WithMessage("Player not found");
        }
    }
}
