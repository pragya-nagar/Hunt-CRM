using System;
using Synergy.Common.DAL.Abstract;

namespace Synergy.CRM.DAL.Queries.Entities
{
    public class OpportunityAudit : IAuditEntity<Guid>, IEntity
    {
        public int OpportunityStageId { get; set; }

        public int? LoanTypeId { get; set; }

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

        public int OpportunityPropertyTypeId { get; set; }

        public string OpportunityNumber { get; set; }

        public Guid Id { get; set; }

        public DateTime CreatedOn { get; set; }

        public Guid CreatedById { get; set; }

        public DateTime ModifiedOn { get; set; }

        public Guid ModifiedById { get; set; }

        public DateTime InsertedOn { get; set; }

        public DateTime? DeletedOn { get; set; }
    }
}
