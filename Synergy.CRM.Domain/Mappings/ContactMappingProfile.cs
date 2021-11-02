using AutoMapper;
using Synergy.CRM.Models;
using Synergy.CRM.Models.Commands;

namespace Synergy.CRM.Domain.Mappings
{
    public class ContactMappingProfile : Profile
    {
        public ContactMappingProfile()
        {
            this.CreateMap<DAL.Queries.Original.Models.ContactModel, ContactModel>();

            this.CreateMap<DAL.Queries.Original.Models.ContactModel, ContactDetailsModel>();

            this.CreateMap<DAL.Queries.Original.Models.ContactAddressModel, AddressModel>()
                .ForMember(x => x.Address1, exp => exp.MapFrom(x => x.Address1))
                .ForMember(x => x.Address2, exp => exp.MapFrom(x => x.Address2))
                .ForMember(x => x.Address3, exp => exp.MapFrom(x => x.Address3));

            this.CreateMap<AddressCreateArgs, AddressArg>()
                .ForMember(x => x.Address1, exp => exp.MapFrom(x => x.Address1))
                .ForMember(x => x.Address2, exp => exp.MapFrom(x => x.Address2))
                .ForMember(x => x.Address3, exp => exp.MapFrom(x => x.Address3));
        }
    }
}
