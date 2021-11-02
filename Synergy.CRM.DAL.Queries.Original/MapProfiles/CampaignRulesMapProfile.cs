using AutoMapper;
using Synergy.CRM.DAL.Queries.Original.Models;
using Synergy.DataAccess.Entities;

namespace Synergy.CRM.DAL.Queries.Original.MapProfiles
{
    public class CampaignRulesMapProfile : Profile
    {
        public CampaignRulesMapProfile()
        {
            this.CreateMap<CampaignRuleCampaign, CampaignRulesModel>()
                .ForMember(e => e.RuleId, src => src.MapFrom(x => x.CampaignRuleId))
            .ForMember(e => e.CampaignId, src => src.MapFrom(x => x.Campaign.Id))
               .ForMember(e => e.CampaignRuleItems, src => src.MapFrom(x => x.CampaignRule.CampaignRuleItems))
               ;
        }
    }
}