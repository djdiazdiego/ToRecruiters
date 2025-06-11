using Core.Application.Persistence;
using FluentValidation;
using PlayerHub.Application.Pesistence.ReadRepositories;
using PlayerHub.Domain;

namespace PlayerHub.Application.Features.Commands
{
    public sealed class UpdatePlayerCommandValidator : AbstractValidator<UpdatePlayerCommand>
    {
        public UpdatePlayerCommandValidator(IReadUnitOfWork readUnitOfWork)
        {
            RuleFor(x => x.Body.Id).NotEqual(default(Guid)).WithMessage("Invalid user id")
                .MustAsync((x, id, ct) =>
                {
                    var repository = (IPlayerReadRepository)readUnitOfWork.GetRepository<Player>();
                    return repository.ExistsAsync(id, ct);
                })
                .WithMessage("Player not found");
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
