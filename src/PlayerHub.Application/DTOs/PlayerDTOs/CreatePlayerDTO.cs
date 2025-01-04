using Core.BaseDTOs;
using PlayerHub.Domain;

namespace PlayerHub.Application.DTOs.PlayerDTOs
{
    public sealed class CreatePlayerDTO : DTO
    {
        public string Name { get; set; } = string.Empty;
        public PositionValue Position { get; set; }
        public List<SkillValue> Skills { get; set; } = [];
    }
}
