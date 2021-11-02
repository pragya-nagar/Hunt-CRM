using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using Synergy.CRM.DAL.Commands.Models.Results.MailMerge;

namespace Synergy.CRM.Services.Mappings
{
    public class MergeSingleFieldsProfile : Profile
    {
        public MergeSingleFieldsProfile()
        {
            this.CreateMap<MailMergePropertyModel, MergeSingleFields>(MemberList.Source)
                .ForMember(x => x.InternalDelinquencyId, x => x.MapFrom(property => property.InternalPropertyId.ToString()))

                .ForMember(x => x.CampaignName, x => x.MapFrom(property => property.Campaign.CampaignName))
                .ForMember(x => x.CampaignType, x => x.MapFrom(property => property.Campaign.CampaignType))
                .ForMember(x => x.CampaignSubType, x => x.MapFrom(property => property.Campaign.CampaignSubType))
                .ForMember(x => x.CreatedDate, x => x.MapFrom(property => property.Campaign.CreatedDate))
                .ForMember(x => x.Description, x => x.MapFrom(property => property.Campaign.Description))
                .ForMember(x => x.TargetDate, x => x.MapFrom(property => property.Campaign.TargetDate))
                .ForMember(x => x.Note, x => x.MapFrom(property => property.Campaign.Note))
                .ForMember(x => x.AssignedUser, x => x.MapFrom(property => property.Campaign.AssignedUser))
                .ForMember(x => x.State, x => x.MapFrom(property => property.PropertyState))

                .ForSourceMember(x => x.Campaign, x => x.DoNotValidate())
                .ForSourceMember(x => x.PropertyStateId, x => x.DoNotValidate())
                ;
        }
    }
}
