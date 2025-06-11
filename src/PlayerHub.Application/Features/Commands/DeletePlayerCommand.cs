using Core.Application.CQRS;
using Core.Wrappers;

namespace PlayerHub.Application.Features.Commands
{
    public sealed class DeletePlayerCommand(Guid id) : ICommand<Response>
    {
        public Guid Id { get; } = id;
    }
}
