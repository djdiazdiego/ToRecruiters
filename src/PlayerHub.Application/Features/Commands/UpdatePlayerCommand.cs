using Core.Application.CQRS;
using Core.Wrappers;
using PlayerHub.Application.DTOs.PlayerDTOs;

namespace PlayerHub.Application.Features.Commands
{
    public sealed class UpdatePlayerCommand(UpdatePlayerDTO dto) : ICommand<Response<PlayerDTO>>
    {
        public UpdatePlayerDTO Body { get; } = dto;
    }
}
