using System.Collections.Generic;
using AutoMapper;
using Synergy.CRM.DAL.Commands.Models;
using Synergy.DataAccess.Entities;

namespace Synergy.CRM.DAL.Commands.MapProfiles
{
    public class AddLeadsToCampaignMapProfile : Profile
    {
        public AddLeadsToCampaignMapProfile()
        {
            this.CreateMap<AddLeadsToCampaignModel, IEnumerable<CampaignLead>>()
                .ConvertUsing((dst, src) =>
                {
                    var srcList = new List<CampaignLead>();
                    foreach (var leadId in dst.CampaignLeadsIds)
                    {
                        srcList.Add(new CampaignLead { LeadId = leadId,  CampaignId = dst.CampaignId });
                    }

                    return srcList;
                });
        }
    }
}
