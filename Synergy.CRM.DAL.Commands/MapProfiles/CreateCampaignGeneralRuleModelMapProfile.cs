using AutoMapper;
using Synergy.CRM.DAL.Commands.Models;
using Synergy.DataAccess.Entities;

namespace Synergy.CRM.DAL.Commands.MapProfiles
{
    public class CreateCampaignGeneralRuleModelMapProfile : Profile
    {
        public CreateCampaignGeneralRuleModelMapProfile()
        {
            this.CreateMap<CreateRuleModel, CampaignRule>()
                .IgnoreAuditMembers()
                .ForMember(r => r.CampaignRuleItems, src => src.MapFrom(r => r.CampaignRuleItems))
                ;

            this.CreateMap<RuleItemModel, CampaignRuleItem>()
                .IgnoreAuditMembers()
                .ForMember(r => r.CampaignRuleId, src => src.Ignore())
                .ForMember(r => r.CampaignRule, src => src.Ignore())
                .ForMember(r => r.CampaignLogicType, src => src.Ignore())
                .ForMember(r => r.CampaignRuleField, src => src.Ignore())
                ;
        }
    }
}
