using AutoMapper;
using PlayerHub.Application.DTOs;
using PlayerHub.Domain;

namespace PlayerHub.Application.Mappers
{
    internal sealed class SkillProfile : Profile
    {
        public SkillProfile()
        {
            CreateMap<Skill, SkillDTO>();
        }
    }
}
