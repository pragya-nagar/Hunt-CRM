using AutoMapper;
using Synergy.CRM.DAL.Commands.Models;
using Synergy.DataAccess.Entities;

namespace Synergy.CRM.DAL.Commands.MapProfiles
{
    public class ApplyRulesMapProfile : Profile
    {
        public ApplyRulesMapProfile()
        {
            this.CreateMap<CampaignRuleCampaign, CampaignRulesModel>()
                .ForMember(r => r.RuleId, src => src.MapFrom(r => r.Id))
                .ForMember(r => r.CampaignId, src => src.MapFrom(r => r.CampaignId))
                .ForMember(r => r.CampaignRuleItems, src => src.MapFrom(r => r.CampaignRule.CampaignRuleItems))
                ;

            this.CreateMap<CampaignRuleItem, ApplyRulesPropertyItemModel>()
                .ForMember(r => r.LogicType, src => src.MapFrom(r => r.CampaignLogicTypeId))
                .ForMember(r => r.RuleField, src => src.MapFrom(r => r.CampaignRuleFieldId))
                ;
        }
    }
}
