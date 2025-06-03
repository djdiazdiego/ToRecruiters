using Core.Application.Persistence;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using PlayerHub.Domain;

namespace PlayerHub.Application.Commands
{
    public sealed class DeletePlayerCommandValidator : AbstractValidator<DeletePlayerCommand>
    {
        public DeletePlayerCommandValidator(IWriteUnitOfWork writeUnitOfWork)
        {
            RuleFor(x => x.Id).NotEqual(default(Guid)).WithMessage("Invalid user id")
                .MustAsync((x, id, ct) =>
                {
                    var repository = writeUnitOfWork.GetRepository<Player>();
                    return repository.GetQuery().AnyAsync(y => y.Id == id, ct);
                })
                .WithMessage("User not found");
        }
    }
}
