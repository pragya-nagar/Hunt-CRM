using System;

namespace Synergy.CRM.Models
{
    public class PdfExportModel
    {
        public string BorrowersName { get; set; }

        public string PropertyAddress { get; set; }

        public string PreparationDate { get; set; }

        public decimal? InterestRate { get; set; }

        public decimal? PaymentAmount { get; set; }

        public string EstimatedClosingDate { get; set; }

        public string TaxLoanTerm { get; set; }

        public string FirstPaymentDate { get; set; }

        public decimal? EstimatedCountyTaxDue { get; set; }

        public decimal? EstimatedIsdTaxDue { get; set; }

        public decimal? EstimatedMudTaxDue { get; set; }

        public decimal? EstimatedOtherTaxDue { get; set; }

        public decimal? AttorneyCountyFee { get; set; }

        public decimal? AttorneyIsdFee { get; set; }

        public decimal? CountyConstableFees { get; set; }

        public decimal? TotalEstimatedTaxDisbursements { get; set; }

        public decimal? BaseClosingCost { get; set; }

        public decimal? OrganisationPercent { get; set; }

        public decimal? LenderCredit { get; set; }

        public decimal? TotalEstimatedClosingCost { get; set; }

        public decimal? TotalAboveEstimatedDisbursmentTax { get; set; }

        public decimal? TotalAboveClosingCost { get; set; }

        public decimal? PrepayPenalty { get; set; }

        public decimal? EstimatedTaxLoanAmount { get; set; }
    }
}
