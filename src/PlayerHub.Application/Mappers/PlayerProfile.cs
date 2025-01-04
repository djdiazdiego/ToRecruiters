using AutoMapper;
using PlayerHub.Application.DTOs.PlayerDTOs;
using PlayerHub.Domain;

namespace PlayerHub.Application.Mappers
{
    internal sealed class PlayerProfile : Profile
    {
        public PlayerProfile()
        {
            CreateMap<Player, PlayerDTO>()
                .ForMember(x => x.Skills, opt => opt.MapFrom(x => x.Skills.ToList()))
                .ForMember(x => x.Position, opt => opt.MapFrom(x => x.Position.ToString()));
        }
    }
}
