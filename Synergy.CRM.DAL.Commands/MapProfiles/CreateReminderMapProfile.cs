using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using Synergy.CRM.DAL.Commands.Models;
using Synergy.DataAccess.Entities;

namespace Synergy.CRM.DAL.Commands.MapProfiles
{
    public class CreateReminderMapProfile : Profile
    {
        public CreateReminderMapProfile()
        {
            this.CreateMap<CreateReminderModel, Reminder>()
                .IgnoreAuditMembers()
                .ForMember(e => e.User, t => t.Ignore())
                .ForMember(e => e.Contact, t => t.Ignore())
                .ForMember(e => e.Id, t => t.Ignore())
                .ForMember(e => e.InstantPushNotificationSentDateTime, t => t.Ignore())
                .ForMember(e => e.PushNotificationSentDateTime, t => t.Ignore())
                .ForMember(e => e.EmailNotificationSentDateTime, t => t.Ignore());

            this.CreateMap<UpdateReminderNotificationModel, Reminder>()
                .IgnoreAuditMembers()
                .ForMember(e => e.User, t => t.Ignore())
                .ForMember(e => e.Contact, t => t.Ignore())
                .ForMember(e => e.Id, t => t.Ignore())
                .ForMember(e => e.InstantPushNotificationSentDateTime, t => t.Ignore())
                .ForMember(e => e.PushNotificationSentDateTime, t => t.Ignore())
                .ForMember(e => e.EmailNotificationSentDateTime, t => t.Ignore())
                .ForMember(e => e.UserId, t => t.Ignore())
                .ForMember(e => e.LeadId, t => t.Ignore())
                .ForMember(e => e.OpportunityId, t => t.Ignore())
                .ForMember(e => e.ContactId, t => t.Ignore())
                .ForMember(e => e.Description, t => t.Ignore())
                .ForMember(e => e.SheduledDate, t => t.Ignore())
                .ForMember(e => e.SheduledTime, t => t.Ignore())
                .ForMember(e => e.IsRead, t => t.Ignore());
        }
    }
}
