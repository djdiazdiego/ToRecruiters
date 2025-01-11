using Core.BaseDTOs;

namespace PlayerHub.Application.DTOs
{
    public sealed class SkillDTO : DTO
    {
        public string Name { get; set; } = string.Empty;
        public int Value { get; set; }
    }
}
