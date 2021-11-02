using AutoMapper;
using Synergy.CRM.DAL.Queries.Original.Models;
using Synergy.DataAccess.Abstractions;
using Synergy.DataAccess.Abstractions.Models;
using Synergy.DataAccess.Entities;

namespace Synergy.CRM.DAL.Queries.Original.MapProfiles
{
    public class CampaignGeneralRuleMapProfile : Profile
    {
        public CampaignGeneralRuleMapProfile()
        {
            this.CreateMap<CampaignRule, CampaignGeneralRuleModel>()
                .ApplyAuditMembers()
                .ForMember(r => r.CampaignRuleItems, src => src.MapFrom(r => r.CampaignRuleItems));

            this.CreateMap<CampaignRuleItem, CampaignRuleItemModel>()
                .ForMember(l => l.CampaignLogicType, src => src.MapFrom(l => new FastEntityModel<int>
                {
                    Id = l.CampaignLogicType.Id,
                    Name = l.CampaignLogicType.Description,
                }))
                .ForMember(f => f.CampaignRuleField, src => src.MapFrom(f => new FastEntityModel<int>
                {
                    Id = f.CampaignRuleField.Id,
                    Name = f.CampaignRuleField.Description,
                }));
        }
    }
}