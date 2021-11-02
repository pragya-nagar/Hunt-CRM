using AutoMapper;
using Synergy.CRM.DAL.Commands.Models;
using Synergy.DataAccess.Entities;

namespace Synergy.CRM.DAL.Commands.MapProfiles
{
    public class UpdateContactMapProfile : Profile
    {
        public UpdateContactMapProfile()
        {
            this.CreateMap<UpdateContactModel, Contact>()
                .IncludeBase<CreateContactModel, Contact>();
        }
    }
}
