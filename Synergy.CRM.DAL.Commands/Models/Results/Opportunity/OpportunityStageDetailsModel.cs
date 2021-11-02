using System;

namespace Synergy.CRM.DAL.Commands.Models.Results.Opportunity
{
    public class OpportunityStageDetailsModel
    {
        public Guid Id { get; set; }

        public string OpportunityNumber { get; set; }

        public int OpportunityStageId { get; set; }
    }
}
