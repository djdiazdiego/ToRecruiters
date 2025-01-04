using Core.Head.CQRS;
using Core.Head.Wrappers;
using PlayerHub.Application.DTOs.PlayerDTOs;

namespace PlayerHub.Application.Commands
{
    public sealed class CreatePlayerCommand(CreatePlayerDTO dto) : ICommand<Response<PlayerDTO>>
    {
        public CreatePlayerDTO Body { get; } = dto;
    }
}
