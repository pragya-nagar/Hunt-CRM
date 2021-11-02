using System;
using AutoMapper;
using Synergy.CRM.DAL.Queries.Original.Models;
using Synergy.DataAccess.Abstractions;
using Synergy.DataAccess.Abstractions.Models;
using Synergy.DataAccess.Entities;

namespace Synergy.CRM.DAL.Queries.Original.MapProfiles
{
    public class ContactMapProfile : Profile
    {
        public ContactMapProfile()
        {
            this.CreateMap<Contact, ContactAddressModel>()
                .ForMember(e => e.State, t => t.MapFrom(src => src.MailingState))
                .ForMember(e => e.City, t => t.MapFrom(src => src.MailingCity))
                .ForMember(e => e.Zip, t => t.MapFrom(src => src.MailingZipCode))
                .ForMember(e => e.Address1, t => t.MapFrom(src => src.MailingAddress1))
                .ForMember(e => e.Address2, t => t.MapFrom(src => src.MailingAddress2))
                .ForMember(e => e.Address3, t => t.MapFrom(src => src.MailingAddress3))
                .ApplyAuditMembers()
                ;

            this.CreateMap<Lead, FastEntityModel<Guid>>()
                .ForMember(x => x.Id, exp => exp.MapFrom(x => x.Id))
                .ForMember(x => x.Name, exp => exp.MapFrom(x => x.AccountName));

            this.CreateMap<Contact, ContactModel>()
                .ForMember(e => e.Type, t => t.MapFrom(src => src.ContactTypeId))
                .ForMember(e => e.Address, t => t.MapFrom(src => src))
                .ApplyAuditMembers()
                ;
        }
    }
}
