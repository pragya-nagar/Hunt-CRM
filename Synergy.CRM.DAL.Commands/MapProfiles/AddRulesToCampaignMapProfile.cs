using System.Collections.Generic;
using AutoMapper;
using Synergy.CRM.DAL.Commands.Models;
using Synergy.DataAccess.Entities;

namespace Synergy.CRM.DAL.Commands.MapProfiles
{
    public class AddRulesToCampaignMapProfile : Profile
    {
        public AddRulesToCampaignMapProfile()
        {
            this.CreateMap<AddRulesToCampaignModel, IEnumerable<CampaignRuleCampaign>>()
                .ConvertUsing((dst, src) =>
                {
                    var srcList = new List<CampaignRuleCampaign>();
                    foreach (var ruleId in dst.CampaignRuleIds)
                    {
                        srcList.Add(new CampaignRuleCampaign { CampaignRuleId = ruleId, CampaignId = dst.CampaignId });
                    }

                    return srcList;
                });
        }
    }
}
