using FluentValidation;

namespace PlayerHub.Application.Commands
{
    /// <summary>
    /// Validator for the <see cref="CreatePlayerCommand"/> class.
    /// Ensures that the command's properties meet the required validation rules.
    /// </summary>
    public sealed class CreatePlayerCommandValidator : AbstractValidator<CreatePlayerCommand>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreatePlayerCommandValidator"/> class.
        /// Defines validation rules for the <see cref="CreatePlayerCommand"/>.
        /// </summary>
        public CreatePlayerCommandValidator()
        {
            // Ensure the player's name is not empty.
            RuleFor(x => x.Body.Name)
                .NotEmpty()
                .WithMessage("Player name is required and cannot be empty.");

            // Ensure the player's position is a valid enum value.
            RuleFor(x => x.Body.Position)
                .IsInEnum()
                .WithMessage("The specified position is invalid. Please provide a valid position.");

            // Ensure the player's skills list is not empty and contains valid enum values.
            RuleFor(x => x.Body.Skills)
                .NotEmpty()
                .WithMessage("At least one skill is required.")
                .ForEach(x =>
                {
                    x.IsInEnum()
                     .WithMessage("One or more skills are invalid. Please provide valid skills.");
                });
        }
    }
}
