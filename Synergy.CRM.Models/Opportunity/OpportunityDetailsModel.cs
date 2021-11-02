namespace Synergy.CRM.Models.Opportunity
{
    public class OpportunityDetailsModel : OpportunityModel
    {
        public decimal? CloseProbabilityPercent { get; set; }

        public decimal? OriginationPercent { get; set; }

        public decimal? LenderCredit { get; set; }

        public decimal? CurrentLoanBalance { get; set; }

        public decimal? ThirdPartyLoanBalance { get; set; }

        public int? Term { get; set; }

        public decimal? PrePay { get; set; }

        public int? MonthlyPrepay { get; set; }

        public int? PercentagePrepay { get; set; }
    }
}
