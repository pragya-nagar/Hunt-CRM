using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using AutoMapper;
using Synergy.Common.Domain.Models.Common;

namespace Synergy.CRM.Domain.Mappings
{
    public class CommonMappingProfile : Profile
    {
        public CommonMappingProfile()
        {
            this.CreateMap<Enum, FastEntityModel<int>>()
                .ForMember(x => x.Id, exp => exp.MapFrom(x => Convert.ToInt32(x, CultureInfo.InvariantCulture)))
                .ForMember(x => x.Name, exp => exp.MapFrom(x => GetDescription(x)));

            this.CreateMap<DataAccess.Abstractions.Models.FastEntityModel<int>, FastEntityModel<int>>();
        }

        public static string GetDescription(Enum value)
        {
            return value.GetType().GetMember(value.ToString()).FirstOrDefault()?.GetCustomAttribute<DescriptionAttribute>()?.Description ?? value.ToString();
        }
    }
}
