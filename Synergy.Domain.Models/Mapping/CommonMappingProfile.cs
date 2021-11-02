using AutoMapper;
using Synergy.Domain.Models.Common;
using System;

namespace Synergy.Domain.Models.Mapping
{
    public class CommonMappingProfile : Profile
    {
        public CommonMappingProfile()
        {
            this.CreateMap<Enum, FastEntityModel<int>>()
                 .ForMember(x => x.Id, exp => exp.MapFrom(x => Convert.ToInt32(x)))
                 .ForMember(x => x.Name, exp => exp.MapFrom(x => x.ToString()));
        }
    }
}
