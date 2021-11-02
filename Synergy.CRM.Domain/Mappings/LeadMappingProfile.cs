using AutoMapper;
using Synergy.CRM.Models;

namespace Synergy.CRM.Domain.Mappings
{
    public class LeadMappingProfile : Profile
    {
        public LeadMappingProfile()
        {
            this.CreateMap<DAL.Queries.Original.Models.LeadModel, LeadModel>();

            this.CreateMap<DAL.Queries.Original.Models.LeadModel, LeadDetailsModel>();

            this.CreateMap<DAL.Queries.Original.Models.LeadAddressModel, AddressModel>();

            this.CreateMap<DAL.Queries.Original.Models.LeadCommentModel, LeadCommentModel>();
        }
    }
}
