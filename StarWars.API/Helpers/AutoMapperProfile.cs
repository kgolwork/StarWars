using AutoMapper;
using StarWars.API.Dtos;
using StarWars.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StarWars.API.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            //Domain to Dto
            CreateMap<Character, CharacterToAddOrUpdateDto>();
            CreateMap<Character, CharacterDto>()
                .ForMember(dest => dest.Friends, opt =>
                {
                    opt.MapFrom(src => src.Friends.Select(f => f.Friend.Name));
                })
                .ForMember(dest => dest.Episodes, opt =>
                {
                    opt.MapFrom(src => src.Episodes.Select(f => f.Episode.Name));
                });

            CreateMap(typeof(QueryResult<>), typeof(QueryResultDto<>));

            //Dto to Domain
            CreateMap<CharacterToAddOrUpdateDto, Character>();
            CreateMap<CharacterDto, Character>();
            CreateMap<CharacterQueryDto, CharacterQuery>();
        }
    }
}
