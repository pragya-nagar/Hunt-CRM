using System;
using AutoMapper;
using Synergy.CRM.DAL.Queries.Original.Models;
using Synergy.DataAccess.Abstractions.Models;
using Synergy.DataAccess.Entities;

namespace Synergy.CRM.DAL.Queries.Original.MapProfiles
{
    public class ReminderMapProfile : Profile
    {
        public ReminderMapProfile()
        {
            this.CreateMap<Reminder, ReminderModel>()
                .ForMember(e => e.NewStatus, t => t.Ignore())
                .ForMember(e => e.NotificationType, t => t.Ignore())
                .ForMember(om => om.Contact, src => src.MapFrom(x =>
                    new FastEntityModel<Guid>
                    {
                        Id = x.Contact.Id,
                        Name = x.Contact.FirstName + " " + x.Contact.MiddleName + " " + x.Contact.LastName,
                    }))
                ;
        }
    }
}
