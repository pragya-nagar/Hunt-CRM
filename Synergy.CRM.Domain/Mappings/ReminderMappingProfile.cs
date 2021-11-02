using System;
using AutoMapper;
using Synergy.Common.Domain.Models.Common;
using Synergy.CRM.Models;

namespace Synergy.CRM.Domain.Mappings
{
    public class ReminderMappingProfile : Profile
    {
        public ReminderMappingProfile()
        {
            this.CreateMap<DAL.Queries.Original.Models.ReminderModel, ReminderModel>();
            this.CreateMap<DAL.Queries.Original.Models.ReminderModel, ReminderDetails>()
                .ForMember(x => x.Status, src => src.MapFrom(x => x.NewStatus));
            this.CreateMap<DAL.Queries.Entities.Lead, FastEntityModel<Guid>>()
                .ForMember(x => x.Name, exp => exp.MapFrom(x => x.AccountName));
            this.CreateMap<DAL.Queries.Entities.Contact, FastEntityModel<Guid>>()
                .ForMember(x => x.Name, exp => exp.MapFrom(x => x.FirstName));
            this.CreateMap<DAL.Queries.Entities.User, FastEntityModel<Guid>>()
                .ForMember(x => x.Name, exp => exp.MapFrom(x => x.FirstName));
        }
    }
}
