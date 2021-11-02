using System;
using System.Linq;
using AutoMapper;
using Synergy.CRM.DAL.Queries.Original.Models;
using Synergy.DataAccess.Abstractions;
using Synergy.DataAccess.Abstractions.Models;
using Synergy.DataAccess.Entities.OpportunityEntities;

namespace Synergy.CRM.DAL.Queries.Original.MapProfiles
{
    public class OpportunityMapProfile : Profile
    {
        public OpportunityMapProfile()
        {
            this.CreateMap<Opportunity, OpportunityModel>()
                .ForMember(om => om.Properties, src => src.MapFrom(x => x.OpportunityProperties.Select(op => op.Property)))
                .ForMember(om => om.LoanOfficer, src => src.MapFrom(x => x.User))
                .ForMember(om => om.LoanType, src => src.MapFrom(x => x.LoanTypeId))
                .ForMember(om => om.OpportunityStage, src => src.MapFrom(x => x.OpportunityStageId))
                .ForMember(x => x.Borrowers, exp => exp.MapFrom(x => x.OpportunityBorrowers))
                .ForMember(x => x.CommercialBorrowers, exp => exp.MapFrom(x => x.OpportunityCommercialBorrowers))
                .ForMember(x => x.LeadSourceId, src => src.MapFrom(x => x.Lead.LeadSourceId))
                .ForMember(x => x.FileDateStarted, src => src.Ignore())

                .ForMember(om => om.Lead, src => src.MapFrom(x =>
                        new FastEntityModel<Guid>
                        {
                            Id = x.Lead.Id,
                            Name = x.Lead.AccountName,
                        }))
                .ApplyAuditMembers()
                ;

            this.CreateMap<DataAccess.Entities.OpportunityEntities.OpportunityBorrower, Models.OpportunityBorrower>();
            this.CreateMap<DataAccess.Entities.OpportunityEntities.OpportunityCommercialBorrower, Models.OpportunityCommercialBorrower>();

            this.CreateMap<DataAccess.Entities.OpportunityEntities.OpportunityBorrower, BorrowerSensetiveData>()
                .ForMember(om => om.TaxIdNumber, src => src.Ignore());

            this.CreateMap<DataAccess.Entities.OpportunityEntities.OpportunityCommercialBorrower, BorrowerSensetiveData>()
                .ForMember(om => om.DateOfBirth, src => src.Ignore())
                .ForMember(om => om.SSN, src => src.Ignore());
        }
    }
}
