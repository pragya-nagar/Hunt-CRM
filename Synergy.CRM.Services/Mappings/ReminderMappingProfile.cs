using AutoMapper;
using Synergy.CRM.DAL.Commands.Models;
using Synergy.CRM.Models.Commands.Reminder;

namespace Synergy.CRM.Services.Mappings
{
    public class ReminderMappingProfile : Profile
    {
        public ReminderMappingProfile()
        {
            this.CreateMap<ReminderCreateCommand, CreateReminderModel>();
            this.CreateMap<ReminderUpdateNotificationCommand, DAL.Commands.Models.UpdateReminderNotificationModel>();
            this.CreateMap<ReminderUpdateCommand, UpdateReminderModel>();
        }
    }
}
