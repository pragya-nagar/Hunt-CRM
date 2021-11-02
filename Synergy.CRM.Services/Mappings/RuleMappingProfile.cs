using System;
using AutoMapper;
using Synergy.CRM.DAL.Commands.Models;
using Synergy.CRM.Models.Commands;

namespace Synergy.CRM.Services.Mappings
{
    public class RuleMappingProfile : Profile
    {
        public RuleMappingProfile()
        {
            this.CreateMap<CreateRuleCommand, CreateRuleModel>()
                .ForMember(x => x.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(x => x.Name, opt => opt.MapFrom(src => src.RuleName))
                .ForMember(x => x.CampaignRuleItems, opt => opt.MapFrom(src => src.RuleItems));

            this.CreateMap<RuleItem, RuleItemModel>()
                .ForMember(x => x.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
                .ForMember(x => x.CampaignLogicTypeId, opt => opt.MapFrom(src => src.DataCutLogicTypeId))
                .ForMember(x => x.CampaignRuleFieldId, opt => opt.MapFrom(src => src.DataCutRuleFieldId))
                ;
        }
    }
}
