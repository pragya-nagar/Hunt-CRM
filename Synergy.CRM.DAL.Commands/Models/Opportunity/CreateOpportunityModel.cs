using System;
using System.Collections.Generic;
using Synergy.CRM.DAL.Commands.Models.Opportunity;

namespace Synergy.CRM.DAL.Commands.Models
{
    public class CreateOpportunityModel
    {
        public Guid Id { get; set; }

        public Guid LeadId { get; set; }

        public Guid UserId { get; set; }

        public string OpportunityNumber { get; set; }

        public int OpportunityStageId { get; set; }

        public int? LoanTypeId { get; set; }

        public Guid? CampaignId { get; set; }

        public Guid? ContactId { get; set; }

        public DateTime? CloseDate { get; set; }

        public decimal? CloseProbabilityPercent { get; set; }

        public decimal? LoanToValuePercent { get; set; }

        public decimal? OriginationPercent { get; set; }

        public decimal? ClosingCost { get; set; }

        public decimal? LenderCredit { get; set; }

        public decimal? AmountDue { get; set; }

        public decimal? InterestRate { get; set; }

        public decimal? CurrentLoanBalance { get; set; }

        public decimal? ThirdPartyLoanBalance { get; set; }

        public int? Term { get; set; }

        public decimal? PrePay { get; set; }

        public int? MonthlyPrepay { get; set; }

        public int? PercentagePrepay { get; set; }

        public List<Guid> Properties { get; set; }

        public int OpportunityPropertyTypeId { get; set; }

        public List<OpportunityBorrower> OpportunityBorrowers { get; set; }

        public List<OpportunityCommercialBorrower> OpportunityCommercialBorrowers { get; set; }

        public string CurrentMilestone { get; set; }
    }
}
