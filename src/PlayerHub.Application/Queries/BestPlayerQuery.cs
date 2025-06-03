using Core.Application.CQRS;
using Core.Wrappers;
using PlayerHub.Application.DTOs.PlayerDTOs;

namespace PlayerHub.Application.Queries
{
    public sealed class BestPlayerQuery(List<BestPlayerRequestDTO> dto) : IQuery<Response<List<BestPlayerDTO>>>
    {
        public List<BestPlayerRequestDTO> Data { get; } = dto;
    }
}
