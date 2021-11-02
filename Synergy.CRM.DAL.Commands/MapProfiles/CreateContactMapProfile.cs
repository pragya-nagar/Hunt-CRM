using AutoMapper;
using Synergy.CRM.DAL.Commands.Models;
using Synergy.DataAccess.Entities;

namespace Synergy.CRM.DAL.Commands.MapProfiles
{
    public class CreateContactMapProfile : Profile
    {
        public CreateContactMapProfile()
        {
            this.CreateMap<CreateContactModel, Contact>()
                .IgnoreAuditMembers()
                .ForMember(e => e.MailingStateId, t => t.MapFrom(src => src.Address.StateId))
                .ForMember(e => e.MailingCity, t => t.MapFrom(src => src.Address.City))
                .ForMember(e => e.MailingZipCode, t => t.MapFrom(src => src.Address.Zip))
                .ForMember(e => e.MailingAddress1, t => t.MapFrom(src => src.Address.Address1))
                .ForMember(e => e.MailingAddress2, t => t.MapFrom(src => src.Address.Address2))
                .ForMember(e => e.MailingAddress3, t => t.MapFrom(src => src.Address.Address3))
                .ForMember(c => c.ContactType, src => src.Ignore())
                .ForMember(c => c.ContactTypeId, src => src.MapFrom(c => c.TypeId))
                .ForMember(c => c.Lead, src => src.Ignore())
                .ForMember(c => c.LeadId, src => src.MapFrom(c => c.LeadId))
                .ForMember(c => c.OpportunityContacts, src => src.Ignore())
                .ForMember(c => c.MailingState, src => src.Ignore())
                ;
        }
    }
}
