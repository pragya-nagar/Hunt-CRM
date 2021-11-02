using AutoMapper;
using Synergy.CRM.DAL.Queries.Original.Models;
using Synergy.DataAccess.Entities;

namespace Synergy.CRM.DAL.Queries.Original.MapProfiles
{
    public class ApplyRuleMapProfile : Profile
    {
        public ApplyRuleMapProfile()
        {
            this.CreateMap<Property, PropertyApplyRulesModel>()
                 .ForMember(e => e.LeadId, t => t.MapFrom(src => src.LeadId))
                 .ForMember(e => e.TotalAmountDue, t => t.MapFrom(src => src.Lead.TotalAmountDueProperties))
                ;
            this.CreateMap<CampaignRuleItem, ApplyRulesPropertyItemModel>()
                .ForMember(l => l.LogicType, exp => exp.MapFrom(src => src.CampaignLogicTypeId))
                 .ForMember(l => l.RuleField, exp => exp.MapFrom(src => src.CampaignRuleFieldId))
                 ;
        }
    }
}
