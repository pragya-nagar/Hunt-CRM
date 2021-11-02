using System.Linq;
using AutoMapper;
using Synergy.CRM.DAL.Queries.Original.Models;
using Synergy.DataAccess.Abstractions;
using Synergy.DataAccess.Entities;

namespace Synergy.CRM.DAL.Queries.Original.MapProfiles
{
    public class LeadModelMapProfile : Profile
    {
        public LeadModelMapProfile()
        {
            this.CreateMap<Lead, LeadModel>()
                .ForMember(e => e.Address, t => t.MapFrom(src => src))
                .ForMember(e => e.PropertiesCount, t => t.MapFrom(src => src.Properties.Count()))
                .ForMember(e => e.TotalTaxDue, t => t.MapFrom(x => x.TotalAmountDueProperties))
                .ApplyAuditMembers()
                    ;
            this.CreateMap<Lead, LeadAddressModel>()
                .ForMember(e => e.State, t => t.MapFrom(src => src.MailingState))
                .ForMember(e => e.City, t => t.MapFrom(src => src.MailingCity))
                .ForMember(e => e.Zip, t => t.MapFrom(src => src.MailingZipCode))
                .ForMember(e => e.Address1, t => t.MapFrom(src => src.MailingAddress1))
                .ForMember(e => e.Address2, t => t.MapFrom(src => src.MailingAddress2))
                .ForMember(e => e.Address3, t => t.MapFrom(src => src.MailingAddress3))
                .ApplyAuditMembers()
                ;
        }
    }
}
