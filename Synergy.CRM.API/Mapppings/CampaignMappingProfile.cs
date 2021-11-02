using System;
using AutoMapper;
using Synergy.Common.Domain.Models.Common;
using Synergy.CRM.Models;
using Synergy.CRM.Models.Commands;

namespace Synergy.CRM.API.Mapppings
{
    public class CampaignMappingProfile : Profile
    {
        public CampaignMappingProfile()
        {
            this.CreateMap<CampaignCreateArgs, CampaignCreateCommand>()
                .ForMember(x => x.Id, exp => exp.Ignore())
                .ForMember(x => x.CreatedOn, exp => exp.Ignore())
                .ForMember(x => x.CreatedBy, exp => exp.Ignore());

            this.CreateMap<CampaignUpdateArgs, CampaignUpdateCommand>()
                .ForMember(x => x.Id, exp => exp.Ignore())
                .ForMember(x => x.CreatedOn, exp => exp.Ignore())
                .ForMember(x => x.CreatedBy, exp => exp.Ignore());
        }
    }
}
