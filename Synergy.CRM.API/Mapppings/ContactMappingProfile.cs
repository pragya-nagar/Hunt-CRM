using AutoMapper;
using Synergy.CRM.Models;
using Synergy.CRM.Models.Commands;

namespace Synergy.CRM.API.Mappings
{
    public class ContactMappingProfile : Profile
    {
        public ContactMappingProfile()
        {
            this.CreateMap<ContactCreateArgs, ContactCreateCommand>()
                .ForMember(x => x.Id, exp => exp.Ignore())
                .ForMember(x => x.CreatedOn, exp => exp.Ignore())
                .ForMember(x => x.CreatedBy, exp => exp.Ignore())
                .ForMember(x => x.TypeId, exp => exp.MapFrom(x => (int?)x.Type));

            this.CreateMap<ContactUpdateArgs, ContactUpdateCommand>()
                .ForMember(x => x.Id, exp => exp.Ignore())
                .ForMember(x => x.CreatedOn, exp => exp.Ignore())
                .ForMember(x => x.CreatedBy, exp => exp.Ignore())
                .ForMember(x => x.TypeId, exp => exp.MapFrom(x => (int?)x.Type));
        }
    }
}
