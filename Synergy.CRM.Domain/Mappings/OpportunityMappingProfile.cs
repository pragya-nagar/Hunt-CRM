using System;
using System.Globalization;
using AutoMapper;
using Synergy.Common.Domain.Models.Common;
using Synergy.CRM.Models.Opportunity;

namespace Synergy.CRM.Domain.Mappings
{
    public class OpportunityMappingProfile : Profile
    {
        public OpportunityMappingProfile()
        {
            this.CreateMap<DAL.Queries.Original.Models.OpportunityModel, OpportunityModel>()
                .ForMember(x => x.Stage, exp => exp.MapFrom(x => x.OpportunityStage))
                .ForMember(x => x.Year, exp => exp.MapFrom(x => x.CreatedOn.Year));

            this.CreateMap<DAL.Queries.Original.Models.OpportunityModel, OpportunityDetailsModel>()
                .ForMember(x => x.Stage, exp => exp.MapFrom(x => x.OpportunityStage))
                .ForMember(x => x.Year, exp => exp.MapFrom(x => x.CreatedOn.Year));

            this.CreateMap<DAL.Queries.Original.Models.OpportunityBorrower, OpportunityBorrowerModel>();

            this.CreateMap<DAL.Queries.Original.Models.OpportunityCommercialBorrower, OpportunityCommercialBorrowerModel>();

            this.CreateMap<DAL.Queries.Entities.OpportunityPropertyType, FastEntityModel<int>>()
                .ForMember(x => x.Name, exp => exp.MapFrom(x => x.Description));

            this.CreateMap<DAL.Queries.Entities.OpportunityMonthlyPrepay, FastEntityModel<int>>()
               .ForMember(x => x.Name, exp => exp.MapFrom(x => x.MonthlyPrepay));

            this.CreateMap<DAL.Queries.Entities.OpportunityPercentagePrepay, FastEntityModel<int>>()
               .ForMember(x => x.Name, exp => exp.MapFrom(x => x.PercentagePrepay));
        }
    }
}
