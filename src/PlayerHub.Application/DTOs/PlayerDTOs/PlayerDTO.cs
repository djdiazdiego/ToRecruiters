using Core.BaseDTOs;
using PlayerHub.Domain;

namespace PlayerHub.Application.DTOs.PlayerDTOs
{
    public sealed class PlayerDTO : DTO
    {
        public string Name { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public List<SkillDTO> Skills { get; set; } = [];
    }
}
