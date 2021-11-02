using System;
using System.Collections.Generic;
using Synergy.DataAccess.Abstractions.Interfaces;
using Synergy.DataAccess.Abstractions.Models;
using Synergy.DataAccess.Entities;

namespace Synergy.CRM.DAL.Queries.Original.Models
{
    public class PropertyModel : AuditModel, IModel, IEntity
    {
        public Guid Id { get; set; }

        public FastEntityModel<int> County { get; set; }

        public DataAccess.Enum.GeneralLandUseCode GeneralLandUseCode { get; set; }

        public string CADId { get; set; }

        public string TAXId { get; set; }

        public string Address { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string ZipCode { get; set; }

        public string LegalDescription { get; set; }

        public string LandStateCode { get; set; }

        public string ImprovementStateCode { get; set; }

        public string LandUseCode { get; set; }

        public bool Over65SurvivingSpouse { get; set; }

        public bool DisabilityExemption { get; set; }

        public bool Mortgage { get; set; }

        public bool PaymentPlan { get; set; }

        public bool Veteran { get; set; }

        public bool Bankruptcy { get; set; }

        public bool ThirdPartyForeclosure { get; set; }

        public FastEntityModel<Guid> Lead { get; set; }

        public string ParcelId { get; set; }

        public decimal TotalAmountDue { get; set; }

        public decimal LatestYearDue { get; set; }

        public IEnumerable<DelinquencyModel> Delinquencies { get; set; }

        public IEnumerable<PropertyValuationModel> PropertyValuations { get; set; }

        public IEnumerable<ContactModel> Contacts { get; set; }
    }
}
