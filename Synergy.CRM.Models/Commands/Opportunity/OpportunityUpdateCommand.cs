using System;
using System.Collections.Generic;
using Synergy.ServiceBus.Abstracts;

namespace Synergy.CRM.Models.Commands.Opportunity
{
    public class OpportunityUpdateCommand : Command
    {
        public Guid LeadId { get; set; }

        public Guid UserId { get; set; }

        public int? LoanTypeId { get; set; }

        public int StageId { get; set; }

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

        public decimal? PrePay { get; set; }

        public int? MonthlyPrepay { get; set; }

        public int? PercentagePrepay { get; set; }

        public int? Term { get; set; }

        public Guid? CampaignId { get; set; }

        public Guid? ContactId { get; set; }

        public IEnumerable<Guid> PropertyIds { get; set; }

        public List<BorrowerArgs> Borrowers { get; set; }

        public List<CommercialBorrowerArgs> CommercialBorrowers { get; set; }

        public int OpportunityPropertyTypeId { get; set; }

        public string CurrentMilestone { get; set; }
    }
}