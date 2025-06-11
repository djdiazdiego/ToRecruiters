using Core.Application.CQRS;
using Core.Wrappers;
using PlayerHub.Application.DTOs.PlayerDTOs;

namespace PlayerHub.Application.Features.Commands
{
    public sealed class CreatePlayerCommand(CreatePlayerDTO dto) : ICommand<Response<PlayerDTO>>
    {
        public CreatePlayerDTO Body { get; } = dto;
    }
}
