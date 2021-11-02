using AutoMapper;
using Synergy.CRM.Models.Commands;

namespace Synergy.CRM.Services.Mappings
{
    public class CampaignMappingProfile : Profile
    {
        public CampaignMappingProfile()
        {
            this.CreateMap<CampaignCreateCommand, DAL.Commands.Models.CreateCampaignModel>()
                .ForMember(x => x.Id, exp => exp.MapFrom(x => x.Id))
                .ForMember(x => x.CreateDate, exp => exp.MapFrom(x => x.CreatedOn))
                .ForMember(x => x.AssignedUserId, exp => exp.MapFrom(x => x.AssignedToUserId))
                .ForMember(x => x.CampaignTypeId, exp => exp.MapFrom(x => x.TypeId))
                .ForMember(x => x.CampaignSubTypeId, exp => exp.MapFrom(x => x.SubTypeId))
                .ForMember(x => x.CampaignName, exp => exp.MapFrom(x => x.Name))
                .ForMember(x => x.CountyIds, exp => exp.MapFrom(x => x.CountyIds))
                .ForMember(x => x.StateId, exp => exp.MapFrom(x => x.StateId))
                ;

            this.CreateMap<CampaignUpdateCommand, DAL.Commands.Models.UpdateCampaignModel>()
                .ForMember(x => x.Id, exp => exp.MapFrom(x => x.Id))
                .ForMember(x => x.AssignedUserId, exp => exp.MapFrom(x => x.AssignedToUserId))
                .ForMember(x => x.CampaignTypeId, exp => exp.MapFrom(x => x.TypeId))
                .ForMember(x => x.CampaignSubTypeId, exp => exp.MapFrom(x => x.SubTypeId))
                .ForMember(x => x.CampaignName, exp => exp.MapFrom(x => x.Name))
                .ForMember(x => x.CountyIds, exp => exp.MapFrom(x => x.CountyIds))
                .ForMember(x => x.StateId, exp => exp.MapFrom(x => x.StateId))
                ;
        }
    }
}
