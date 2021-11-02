namespace Synergy.CRM.DAL.Queries.Entities
{
    public class OpportunityCommercialBorrower : OpportunityBorrowerBase
    {
        public string EntityName { get; set; }

        public string TaxIdNumber { get; set; }

        public string Title { get; set; }

        public Opportunity Opportunity { get; set; }
    }
}
