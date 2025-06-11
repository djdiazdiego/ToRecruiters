using AutoMapper;
using Mapster;
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

    internal sealed class SkillMapping : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Skill, SkillDTO>();
        }
    }
}
