using System.Linq;
using AutoMapper;
using Synergy.CRM.DAL.Commands.Models;
using Synergy.DataAccess.Entities.OpportunityEntities;

namespace Synergy.CRM.DAL.Commands.MapProfiles
{
    public class CreateOpportunityMapProfile : Profile
    {
        public CreateOpportunityMapProfile()
        {
            this.CreateMap<CreateOpportunityModel, Opportunity>()
                .IgnoreAuditMembers()
                .ForMember(e => e.OpportunityProperties, t => t.MapFrom(p => p.Properties.Select(op => new OpportunityProperty { OpportunityId = p.Id, PropertyId = op })))
                .ForMember(e => e.Lead, t => t.Ignore())
                .ForMember(e => e.OpportunityStage, t => t.Ignore())
                .ForMember(e => e.LoanType, t => t.Ignore())
                .ForMember(e => e.Campaign, t => t.Ignore())
                .ForMember(e => e.Contact, t => t.Ignore())
                .ForMember(e => e.OpportunityContacts, t => t.Ignore())
                .ForMember(e => e.User, t => t.Ignore())
                .ForMember(e => e.OpportunityPropertyType, t => t.Ignore())
                ;

            this.CreateMap<Models.Opportunity.OpportunityBorrower, OpportunityBorrower>()
                .IgnoreAuditMembers()
                .ForMember(e => e.Id, t => t.Ignore())
               .ForMember(e => e.Opportunity, t => t.Ignore())
               .ForMember(e => e.OpportunityId, t => t.Ignore())
               .ForMember(e => e.SSN, t => t.Ignore())
               .ForMember(e => e.DateOfBirth, t => t.Ignore());

            this.CreateMap<Models.Opportunity.OpportunityCommercialBorrower, OpportunityCommercialBorrower>()
                .IgnoreAuditMembers()
                .ForMember(e => e.Id, t => t.Ignore())
                .ForMember(e => e.Opportunity, t => t.Ignore())
                .ForMember(e => e.OpportunityId, t => t.Ignore())
                .ForMember(e => e.TaxIdNumber, t => t.Ignore());
        }
    }
}
