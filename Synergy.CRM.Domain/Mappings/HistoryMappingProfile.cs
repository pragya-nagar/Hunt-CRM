using AutoMapper;
using Synergy.CRM.DAL.Queries.Entities;
using Synergy.CRM.Models.Opportunity;

namespace Synergy.CRM.Domain.Mappings
{
    public class HistoryMappingProfile : Profile
    {
        public HistoryMappingProfile()
        {
            CreateMap<Opportunity, OpportunityAudit>()
                .ForMember(o => o.InsertedOn, src => src.Ignore());

            CreateMap<OpportunityBorrower, OpportunityBorrowerBaseAudit>()
                .ForMember(o => o.TaxIdNumber, src => src.Ignore())
                .ForMember(o => o.EntityName, src => src.Ignore())
                .ForMember(o => o.Title, src => src.Ignore())
                .ForMember(o => o.InsertedOn, src => src.Ignore())
                .ForMember(o => o.Discriminator, src => src.MapFrom(x => nameof(OpportunityBorrower)));

            CreateMap<OpportunityCommercialBorrower, OpportunityBorrowerBaseAudit>()
                .ForMember(o => o.SSN, src => src.Ignore())
                .ForMember(o => o.IsMarried, src => src.Ignore())
                .ForMember(o => o.DateOfBirth, src => src.Ignore())
                .ForMember(o => o.InsertedOn, src => src.Ignore())
                .ForMember(o => o.Discriminator, src => src.MapFrom(x => nameof(OpportunityCommercialBorrower)));

            CreateMap<Contact, ContactAudit>()
               .ForMember(o => o.InsertedOn, src => src.Ignore());

            CreateMap<Property, PropertyAudit>()
               .ForMember(o => o.InsertedOn, src => src.Ignore());

            CreateMap<OpportunityProperty, OpportunityPropertyHistoryModel>()
                .ForMember(o => o.UpdatedBy, src => src.MapFrom(x => x.ModifiedById));
        }
    }
}
