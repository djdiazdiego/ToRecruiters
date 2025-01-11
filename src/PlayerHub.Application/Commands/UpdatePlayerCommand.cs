using Core.Head.CQRS;
using Core.Head.Wrappers;
using PlayerHub.Application.DTOs.PlayerDTOs;

namespace PlayerHub.Application.Commands
{
    public sealed class UpdatePlayerCommand(UpdatePlayerDTO dto) : ICommand<Response<PlayerDTO>>
    {
        public UpdatePlayerDTO Body { get; } = dto;
    }
}
