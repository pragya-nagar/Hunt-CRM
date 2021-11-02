﻿using AutoMapper;
using Synergy.CRM.DAL.Commands.Models;
using Synergy.DataAccess.Entities;

namespace Synergy.CRM.DAL.Commands.MapProfiles
{
    public class CreateCampaignMapProfile : Profile
    {
        public CreateCampaignMapProfile()
        {
            this.CreateMap<CreateCampaignModel, Campaign>()
                .IgnoreAuditMembers()
                .ForMember(c => c.CampaignType, src => src.Ignore())
                .ForMember(c => c.AssignedUser, src => src.Ignore())
                .ForMember(c => c.CampaignLeads, src => src.Ignore())
                .ForMember(c => c.CampaignSubType, src => src.Ignore())
                .ForMember(c => c.CampaignRuleCampaign, src => src.Ignore())
                .ForMember(c => c.TotalAmountRecords, src => src.Ignore())
                .ForMember(c => c.TotalRecords, src => src.Ignore())
                .ForMember(c => c.CampaignCounty, src => src.Ignore())
                .ForMember(c => c.State, src => src.Ignore())
                ;
        }
    }
}
