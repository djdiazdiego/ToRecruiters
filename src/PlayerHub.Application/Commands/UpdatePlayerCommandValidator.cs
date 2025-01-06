using Core.Data.UnitOfWorks;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using PlayerHub.Domain;

namespace PlayerHub.Application.Commands
{
    public sealed class UpdatePlayerCommandValidator : AbstractValidator<UpdatePlayerCommand>
    {
        public UpdatePlayerCommandValidator(IWriteUnitOfWork writeUnitOfWork)
        {
            RuleFor(x => x.Body.Id).NotEqual(default(Guid)).WithMessage("Invalid user id")
                .MustAsync((x, id, ct) =>
                {
                    var repository = writeUnitOfWork.GetRepository<Player>();
                    return repository.GetQuery().AnyAsync(y => y.Id == id, ct);
                })
                .WithMessage("User not found");
            RuleFor(x => x.Body.Name).NotEmpty().WithMessage("Name is required");
            RuleFor(x => x.Body.Position).IsInEnum().WithMessage("Invalid position");
            RuleFor(x => x.Body.Skills).NotEmpty().WithMessage("Skill is required")
                .ForEach(x =>
                {
                    RuleFor(y => y.Body).IsInEnum().WithMessage("Invalid skill");
                });
        }
    }
}
