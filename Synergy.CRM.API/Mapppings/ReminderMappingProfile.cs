using AutoMapper;
using Synergy.CRM.Models.Commands.Reminder;
using Synergy.CRM.Models.Reminder;

namespace Synergy.CRM.API.Mapppings
{
    public class ReminderMappingProfile : Profile
    {
        public ReminderMappingProfile()
        {
            this.CreateMap<ReminderUpdateArgs, ReminderUpdateCommand>()
                .ForMember(x => x.Id, exp => exp.Ignore())
                .ForMember(x => x.CreatedOn, exp => exp.Ignore())
                .ForMember(x => x.CreatedBy, exp => exp.Ignore());

            this.CreateMap<ReminderCreateArgs, ReminderCreateCommand>()
                .ForMember(x => x.Id, exp => exp.Ignore())
                .ForMember(x => x.CreatedOn, exp => exp.Ignore())
                .ForMember(x => x.CreatedBy, exp => exp.Ignore());

            this.CreateMap<ReminderUpdateNotificationArgs, ReminderUpdateNotificationCommand>()
                .ForMember(x => x.Id, exp => exp.Ignore())
                .ForMember(x => x.CreatedOn, exp => exp.Ignore())
                .ForMember(x => x.CreatedBy, exp => exp.Ignore());
        }
    }
}
