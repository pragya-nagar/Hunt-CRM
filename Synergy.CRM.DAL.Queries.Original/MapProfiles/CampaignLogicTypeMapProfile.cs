using AutoMapper;
using Synergy.CRM.DAL.Queries.Original.Models;
using Synergy.DataAccess.Entities;

namespace Synergy.CRM.DAL.Queries.Original.MapProfiles
{
    public class CampaignLogicTypeMapProfile : Profile
    {
        public CampaignLogicTypeMapProfile()
        {
            this.CreateMap<CampaignLogicType, CampaignLogicTypeModel>()
                .ForMember(e => e.Name, t => t.MapFrom(src => src.Description))
                .ForMember(e => e.FieldDataType, t => t.MapFrom(src => src.CampaignFieldType.Name))
                ;
        }
    }
}
