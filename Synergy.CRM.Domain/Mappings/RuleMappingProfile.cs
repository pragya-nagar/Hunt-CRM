using AutoMapper;
using Synergy.CRM.Models;
using Synergy.CRM.Models.Commands;

namespace Synergy.CRM.Domain.Mappings
{
    public class RuleMappingProfile : Profile
    {
        public RuleMappingProfile()
        {
            this.CreateMap<DAL.Queries.Original.Models.CampaignLogicTypeModel, CampaignLogicTypeModel>();
            this.CreateMap<DAL.Queries.Original.Models.CampaignRuleFieldModel, CampaignRuleFieldModel>();
        }
    }
}
