using Core.BaseDTOs;

namespace PlayerHub.Application.DTOs.PlayerDTOs
{
    public sealed class BestPlayerRequestDTO : DTO
    {
        public string Skill { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
    }
}
