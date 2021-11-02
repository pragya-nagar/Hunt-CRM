using System;
using AutoMapper;
using Synergy.Common.Domain.Models.Common;
using Synergy.CRM.Models;
using Synergy.CRM.Models.Commands;

namespace Synergy.CRM.Domain.Mapppings
{
    public class CampaignMappingProfile : Profile
    {
        public CampaignMappingProfile()
        {
            this.CreateMap<DataAccess.Abstractions.Models.FastEntityModel<Guid>, FastEntityModel<Guid>>();

            this.CreateMap<DAL.Queries.Original.Models.CampaignModel, CampaignModel>()
                .ForMember(x => x.Type, exp => exp.MapFrom(x => x.CampaignType))
                .ForMember(x => x.SubType, exp => exp.MapFrom(x => x.CampaignSubType))
                .ForMember(x => x.Name, exp => exp.MapFrom(x => x.CampaignName));

            this.CreateMap<DAL.Queries.Original.Models.CampaignModel, CampaignDetailsModel>()
                .ForMember(x => x.Type, exp => exp.MapFrom(x => x.CampaignType))
                .ForMember(x => x.SubType, exp => exp.MapFrom(x => x.CampaignSubType))
                .ForMember(x => x.Name, exp => exp.MapFrom(x => x.CampaignName))
                .ForMember(x => x.TotalRecords, exp => exp.MapFrom(x => x.TotalRecords))
                .ForMember(x => x.TotalAmountRecords, exp => exp.MapFrom(x => x.TotalAmountRecords));

            this.CreateMap<DataAccess.Dictionaries.Queries.Models.CampaignTypeModel, CampaignTypeModel>()
                .ForMember(x => x.Children, exp => exp.MapFrom(x => x.CampaingSubType));

            this.CreateMap<DAL.Queries.Original.Models.CampaignCommentModel, CampaignCommentModel>();
        }
    }
}
