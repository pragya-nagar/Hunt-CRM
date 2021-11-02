using System;
using System.Collections.Generic;
using Synergy.Common.Domain.Models.Abstracts;
using Synergy.Common.Domain.Models.Common;

namespace Synergy.CRM.Models.Opportunity
{
    public class OpportunityModel : IResultModel
    {
        public Guid Id { get; set; }

        public int Year { get; set; }

        public string OpportunityNumber { get; set; }

        public FastEntityModel<Guid> Lead { get; set; }

        public ContactModel Contact { get; set; }

        public Guid? CampaignId { get; set; }

        public decimal? AmountDue { get; set; }

        public FastEntityModel<int> Stage { get; set; }

        public FastEntityModel<int> LoanType { get; set; }

        public FastEntityModel<Guid> LoanOfficer { get; set; }

        public IEnumerable<PropertyModel> Properties { get; set; }

        public decimal? LoanToValuePercent { get; set; }

        public DateTime? CloseDate { get; set; }

        public List<OpportunityBorrowerModel> Borrowers { get; set; }

        public List<OpportunityCommercialBorrowerModel> CommercialBorrowers { get; set; }

        public int OpportunityPropertyTypeId { get; set; }

        public string CurrentMilestone { get; set; }

        public decimal? ClosingCost { get; set; }

        public decimal? InterestRate { get; set; }

        public int? LeadSourceId { get; set; }

        public string FileDateStarted { get; set; }
    }
}
