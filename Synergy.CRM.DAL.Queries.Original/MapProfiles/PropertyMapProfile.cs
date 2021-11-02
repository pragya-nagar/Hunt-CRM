using System;
using AutoMapper;
using Synergy.CRM.DAL.Queries.Original.Models;
using Synergy.DataAccess.Abstractions;
using Synergy.DataAccess.Abstractions.Models;
using Synergy.DataAccess.Entities;

namespace Synergy.CRM.DAL.Queries.Original.MapProfiles
{
    public class PropertyMapProfile : Profile
    {
        public PropertyMapProfile()
        {
            this.CreateMap<Property, PropertyModel>()
                .ForMember(e => e.LatestYearDue, t => t.MapFrom(src => src.LastYearDue))
                .ForMember(e => e.GeneralLandUseCode, t => t.MapFrom(src => src.GeneralLandUseCodeId))
                .ForMember(e => e.County, t => t.MapFrom(src => src.County))
                .ForMember(dest => dest.Contacts, opt => opt.MapFrom(x => x.Lead.Contacts))
                .ForMember(e => e.State, opt => opt.MapFrom(x => x.State.Abbreviation))
                .ForMember(dest => dest.Lead, opt => opt.MapFrom(src => new FastEntityModel<Guid>
                                                                        {
                                                                             Id = src.LeadId,
                                                                             Name = src.Lead.AccountName,
                                                                        }))
                .ApplyAuditMembers()
                ;

            this.CreateMap<PropertyValuation, PropertyValuationModel>()
                .ApplyAuditMembers()
                ;
            this.CreateMap<Delinquency, DelinquencyModel>()
                .ForMember(dm => dm.DelinquencyYear, src => src.MapFrom(d => d.DelinquencyTaxYear))
                .ApplyAuditMembers()
                ;

            this.CreateMap<CollectingEntity, CollectingEntityModel>()
               .ApplyAuditMembers();
        }
    }
}
