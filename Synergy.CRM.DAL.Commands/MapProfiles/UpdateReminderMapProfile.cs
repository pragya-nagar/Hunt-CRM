using AutoMapper;
using Synergy.CRM.DAL.Commands.Models;
using Synergy.DataAccess.Entities;

namespace Synergy.CRM.DAL.Commands.MapProfiles
{
    public class UpdateReminderMapProfile : Profile
    {
        public UpdateReminderMapProfile()
        {
            this.CreateMap<UpdateReminderModel, Reminder>()
                .IncludeBase<CreateReminderModel, Reminder>();
        }
    }
}
