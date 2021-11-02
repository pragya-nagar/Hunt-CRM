using System;
using System.Linq;
using AutoMapper;
using Synergy.CRM.DAL.Queries.Original.Models;
using Synergy.DataAccess.Abstractions;
using Synergy.DataAccess.Abstractions.Models;
using Synergy.DataAccess.Entities;

namespace Synergy.CRM.DAL.Queries.Original.MapProfiles
{
    public class CampaignModelMapProfile : Profile
    {
        public CampaignModelMapProfile()
        {
            this.CreateMap<Campaign, CampaignModel>()
                .ForMember(e => e.Counties, t => t.MapFrom(src => src.CampaignCounty.Select(cc =>
                                                new FastEntityModel<int>
                                                {
                                                    Id = cc.CountyId,
                                                    Name = cc.County.Name,
                                                })))
                .ForMember(e => e.State, t => t.MapFrom(src => src.State))
                .ForMember(e => e.TotalRecords, t => t.MapFrom(src => src.TotalRecords ?? 0))
                .ForMember(e => e.TotalAmountRecords, t => t.MapFrom(src => src.TotalAmountRecords ?? 0))
                .ForMember(e => e.AssignedTo, t => t.MapFrom(src =>
                                                new FastEntityModel<Guid>
                                                {
                                                    Id = src.AssignedUserId,
                                                    Name = $"{src.AssignedUser.FirstName}  {src.AssignedUser.LastName}",
                                                }))
                  .ApplyAuditMembers()
                ;
        }
    }
}
