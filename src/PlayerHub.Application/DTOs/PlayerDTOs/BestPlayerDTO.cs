using Core.BaseDTOs;

namespace PlayerHub.Application.DTOs.PlayerDTOs
{
    public sealed class BestPlayerDTO : DTO
    {
        public Guid? Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public SkillDTO Skill { get; set; } = null!;
    }
}
