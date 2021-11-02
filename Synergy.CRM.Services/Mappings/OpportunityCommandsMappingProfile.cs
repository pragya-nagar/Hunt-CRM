using AutoMapper;
using Synergy.CRM.Models.Commands.Opportunity;

namespace Synergy.CRM.Services.Mappings
{
    public class OpportunityCommandsMappingProfile : Profile
    {
        public OpportunityCommandsMappingProfile()
        {
            this.CreateMap<OpportunityCreateCommand, DAL.Commands.Models.CreateOpportunityModel>()
                .ForMember(x => x.OpportunityStageId, exp => exp.MapFrom(x => x.StageId))
                .ForMember(x => x.UserId, exp => exp.MapFrom(x => x.CreatedBy))
                .ForMember(x => x.MonthlyPrepay, exp => exp.MapFrom(x => x.MonthlyPrepay))
                .ForMember(x => x.PercentagePrepay, exp => exp.MapFrom(x => x.PercentagePrepay))
                .ForMember(x => x.LoanTypeId, exp => exp.MapFrom(x => x.LoanTypeId))
                .ForMember(x => x.Properties, exp => exp.MapFrom(x => x.PropertyIds))
                .ForMember(x => x.OpportunityBorrowers, exp => exp.MapFrom(x => x.Borrowers))
                .ForMember(x => x.OpportunityCommercialBorrowers, exp => exp.MapFrom(x => x.CommercialBorrowers))
                .ForMember(x => x.OpportunityNumber, exp => exp.Ignore());

            this.CreateMap<OpportunityUpdateCommand, DAL.Commands.Models.UpdateOpportunityModel>()
                .ForMember(x => x.OpportunityStageId, exp => exp.MapFrom(x => x.StageId))
                .ForMember(x => x.Properties, exp => exp.MapFrom(x => x.PropertyIds))
                .ForMember(x => x.OpportunityBorrowers, exp => exp.MapFrom(x => x.Borrowers))
                .ForMember(x => x.OpportunityCommercialBorrowers, exp => exp.MapFrom(x => x.CommercialBorrowers))
                .ForMember(x => x.OpportunityNumber, exp => exp.Ignore());

            this.CreateMap<BorrowerArgs, DAL.Commands.Models.Opportunity.OpportunityBorrower>();
            this.CreateMap<CommercialBorrowerArgs, DAL.Commands.Models.Opportunity.OpportunityCommercialBorrower>();

            this.CreateMap<OpportunitySensitiveDataUpdateCommand, DAL.Commands.Models.Opportunity.UpdateOpportunitySensitiveDataModel>();

            this.CreateMap<OpportunitySensitiveDataUpdateArgs, DAL.Commands.Models.Opportunity.OpportunitySensitiveDataModel>();
        }
    }
}
