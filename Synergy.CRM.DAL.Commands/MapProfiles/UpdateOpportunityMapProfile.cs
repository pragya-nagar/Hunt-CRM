using AutoMapper;
using Synergy.CRM.DAL.Commands.Models;
using Synergy.DataAccess.Entities.OpportunityEntities;

namespace Synergy.CRM.DAL.Commands.MapProfiles
{
    public class UpdateOpportunityMapProfile : Profile
    {
        public UpdateOpportunityMapProfile()
        {
            this.CreateMap<UpdateOpportunityModel, Opportunity>()
                .IgnoreAuditMembers()
                .ForMember(e => e.OpportunityProperties, t => t.Ignore())
                .ForMember(e => e.OpportunityNumber, t => t.Ignore())
                .ForMember(e => e.OpportunityBorrowers, t => t.Ignore())
                .ForMember(e => e.OpportunityCommercialBorrowers, t => t.Ignore())
                .ForMember(e => e.Lead, t => t.Ignore())
                .ForMember(e => e.OpportunityStage, t => t.Ignore())
                .ForMember(e => e.LoanType, t => t.Ignore())
                .ForMember(e => e.Campaign, t => t.Ignore())
                .ForMember(e => e.Contact, t => t.Ignore())
                .ForMember(e => e.OpportunityContacts, t => t.Ignore())
                .ForMember(e => e.User, t => t.Ignore())
                .ForMember(e => e.OpportunityPropertyType, t => t.Ignore());
        }
    }
}
