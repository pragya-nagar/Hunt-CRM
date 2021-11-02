namespace Synergy.CRM.DAL.Queries.Entities
{
    using System;
    using System.Collections.Generic;
    using Synergy.Common.DAL.Abstract;

    public class Property : IAuditEntity<Guid>
    {
        public string ParcelId { get; set; }

        public string Address { get; set; }

        public string City { get; set; }

        public string ZipCode { get; set; }

        public decimal? TotalAmountDue { get; set; }

        public decimal LastYearDue { get; set; }

        public int StateId { get; set; }

        public int CountyId { get; set; }

        public State State { get; set; }

        public Guid LeadId { get; set; }

        public Lead Lead { get; set; }

        public County County { get; set; }

        public int? GeneralLandUseCodeId { get; set; }

        public GeneralLandUseCode GeneralLandUseCode { get; set; }

        public int? InternalLandUseCodeId { get; set; }

        public InternalLandUseCode InternalLandUseCode { get; set; }

        public IEnumerable<PropertyValuation> Valuations { get; set; }

        public Guid Id { get; set; }

        public DateTime CreatedOn { get; set; }

        public Guid CreatedById { get; set; }

        public DateTime ModifiedOn { get; set; }

        public Guid ModifiedById { get; set; }

        public DateTime? DeletedOn { get; set; }
    }
}
