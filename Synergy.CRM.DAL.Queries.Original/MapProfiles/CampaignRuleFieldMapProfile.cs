using AutoMapper;
using Synergy.CRM.DAL.Queries.Original.Models;
using Synergy.DataAccess.Entities;

namespace Synergy.CRM.DAL.Queries.Original.MapProfiles
{
    public class CampaignRuleFieldMapProfile : Profile
    {
        public CampaignRuleFieldMapProfile()
        {
            this.CreateMap<CampaignRuleField, CampaignRuleFieldModel>()
                .ForMember(e => e.Id, t => t.MapFrom(src => src.Id))
                .ForMember(e => e.Name, t => t.MapFrom(src => src.Description))
                .ForMember(e => e.LogicTypes, t => t.MapFrom(src => src.CampaignFieldType.CampaignLogicTypes))
                ;
        }
    }
}
