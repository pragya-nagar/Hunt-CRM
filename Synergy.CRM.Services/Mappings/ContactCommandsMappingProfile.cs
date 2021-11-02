using AutoMapper;
using Synergy.CRM.Models.Commands;

namespace Synergy.CRM.Services.Mappings
{
    public class ContactCommandsMappingProfile : Profile
    {
        public ContactCommandsMappingProfile()
        {
            this.CreateMap<ContactCreateCommand, DAL.Commands.Models.CreateContactModel>();

            this.CreateMap<ContactUpdateCommand, DAL.Commands.Models.UpdateContactModel>();

            this.CreateMap<AddressArg, DAL.Commands.Models.ContactAddressModel>();
        }
    }
}
