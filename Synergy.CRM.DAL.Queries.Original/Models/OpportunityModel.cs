using System;
using System.Collections.Generic;
using Synergy.DataAccess.Abstractions.Interfaces;
using Synergy.DataAccess.Abstractions.Models;

namespace Synergy.CRM.DAL.Queries.Original.Models
{
    public class OpportunityModel : AuditModel, IModel
    {
        public Guid Id { get; set; }

        public string OpportunityNumber { get; set; }

        public FastEntityModel<Guid> LoanOfficer { get; set; }

        public FastEntityModel<Guid> Lead { get; set; }

        public Guid CampaignId { get; set; }

        public ContactModel Contact { get; set; }

        public DataAccess.Enum.OpportunityStage OpportunityStage { get; set; }

        public DataAccess.Enum.LoanType? LoanType { get; set; }

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

        public List<PropertyModel> Properties { get; set; }

        public int OpportunityPropertyTypeId { get; set; }

        public List<OpportunityBorrower> Borrowers { get; set; }

        public List<OpportunityCommercialBorrower> CommercialBorrowers { get; set; }

        public string CurrentMilestone { get; set; }

        public int? LeadSourceId { get; set; }

        public string FileDateStarted { get; set; }
    }
}
