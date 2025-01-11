using FluentValidation;

namespace PlayerHub.Application.Commands
{
    public sealed class CreatePlayerCommandValidator : AbstractValidator<CreatePlayerCommand>
    {
        public CreatePlayerCommandValidator()
        {
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
