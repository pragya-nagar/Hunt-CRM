using System;
using System.Collections.Generic;
using System.Text;

namespace Synergy.CRM.DAL.Commands.Models.Results.MailMerge
{
    public class MailMergePropertyModel
    {
        public Guid InternalPropertyId { get; set; }

        public string County { get; set; }

        public decimal PropertyAmountDue { get; set; }

        public string ParcelId { get; set; }

        public decimal LeadAmountDue { get; set; }

        public string LandUseCode { get; set; }

        public string GeneralLandUseCode { get; set; }

        public string InternalLandUseCode { get; set; }

        public string Owner { get; set; }

        public string PropertyAddress { get; set; }

        public string PropertyCity { get; set; }

        public string PropertyState { get; set; }

        public string PropertyZipCode { get; set; }

        public decimal AppraisedValue { get; set; }

        public decimal LeadAppraisedValue { get; set; }

        public string MailingAddress1 { get; set; }

        public string MailingAddress2 { get; set; }

        public string MailingAddress3 { get; set; }

        public string MailingCity { get; set; }

        public string MailingState { get; set; }

        public string MailingZipCode { get; set; }

        public bool? DoNotContact { get; set; }

        public MailMergeCampaignModel Campaign { get; set; }

        public decimal LandValue { get; set; }

        public float LandAcres { get; set; }

        public bool? Homestead { get; set; }

        public int BuildingSqFt { get; set; }

        public decimal BuildingValue { get; set; }

        public decimal RUAmount { get; set; }

        public decimal RULTV { get; set; }

        public decimal LTV { get; set; }

        public int PropertyStateId { get; set; }

        public decimal TaxRatio { get; set; }

        public string LegalDescription { get; set; }

        public int? YearBuilt { get; set; }
    }
}