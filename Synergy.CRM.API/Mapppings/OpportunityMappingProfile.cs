using AutoMapper;
using Synergy.Common.Domain.Models.Common;
using Synergy.CRM.Models.Commands.Opportunity;
using Synergy.CRM.Models.Opportunity;
using Synergy.DataAccess.Enum;

namespace Synergy.CRM.API.Mappings
{
    public class OpportunityMappingProfile : Profile
    {
        public OpportunityMappingProfile()
        {
            this.CreateMap<OpportunityCreateArgs, OpportunityCreateCommand>()
                .ForMember(x => x.Id, exp => exp.Ignore())
                .ForMember(x => x.CreatedOn, exp => exp.Ignore())
                .ForMember(x => x.MonthlyPrepay, exp => exp.MapFrom(x => x.MonthlyPrepay ?? 0))
                .ForMember(x => x.PercentagePrepay, exp => exp.MapFrom(x => x.PercentagePrepay ?? 0))
                .ForMember(x => x.CreatedBy, exp => exp.Ignore())
                .ForMember(x => x.ThirdPartyLoanBalance, exp => exp.MapFrom(x => x.ThirdPartyLoanBalance ?? 0))
                .ForMember(x => x.CurrentLoanBalance, exp => exp.MapFrom(x => x.CurrentLoanBalance ?? 0))
                .ForMember(x => x.StageId, exp => exp.MapFrom(x => (int)x.Stage))
                .ForMember(x => x.LoanTypeId, exp => exp.MapFrom(x => (int?)x.LoanType));

            this.CreateMap<OpportunityUpdateArgs, OpportunityUpdateCommand>()
                .ForMember(x => x.Id, exp => exp.Ignore())
                .ForMember(x => x.CreatedOn, exp => exp.Ignore())
                .ForMember(x => x.CreatedBy, exp => exp.Ignore())
                .ForMember(x => x.ThirdPartyLoanBalance, exp => exp.MapFrom(x => x.ThirdPartyLoanBalance ?? 0))
                .ForMember(x => x.CurrentLoanBalance, exp => exp.MapFrom(x => x.CurrentLoanBalance ?? 0))
                .ForMember(x => x.StageId, exp => exp.MapFrom(x => (int)x.Stage))
                .ForMember(x => x.LoanTypeId, exp => exp.MapFrom(x => (int?)x.LoanType));

            this.CreateMap<OpportunityBorrowerCreateArgs, BorrowerArgs>()
                .ForMember(x => x.Id, exp => exp.Ignore());
            this.CreateMap<OpportunityCommercialBorrowerCreateArgs, CommercialBorrowerArgs>()
                .ForMember(x => x.Id, exp => exp.Ignore());

            this.CreateMap<OpportunityBorrowerUpdateArgs, BorrowerArgs>();
            this.CreateMap<OpportunityCommercialBorrowerUpdateArgs, CommercialBorrowerArgs>();

            this.CreateMap<Models.Opportunity.OpportunitySensitiveDataUpdateArgs, Models.Commands.Opportunity.OpportunitySensitiveDataUpdateArgs>();
        }
    }
}
