using AutoMapper;
using Synergy.CRM.Models;
using Synergy.CRM.Models.Commands;

namespace Synergy.CRM.Domain.Mappings
{
    public class RuleMappingProfile : Profile
    {
        public RuleMappingProfile()
        {
            this.CreateMap<CreateRuleArgs, CreateRuleCommand>()
                .ForMember(r => r.Id, src => src.Ignore())
                .ForMember(r => r.CreatedBy, src => src.Ignore())
                .ForMember(r => r.CreatedOn, src => src.Ignore())
                ;

            this.CreateMap<RuleItemArgs, RuleItem>();
        }
    }
}
