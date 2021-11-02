using System;
using System.Collections.Generic;
using Synergy.Common.Domain.Models.Abstracts;
using Synergy.Common.Domain.Models.Common;

namespace Synergy.CRM.Models
{
    public class PropertyDetailsModel : IResultModel
    {
        public Guid Id { get; set; }

        public FastEntityModel<Guid> Lead { get; set; }

        public FastEntityModel<int> County { get; set; }

        public FastEntityModel<int> GeneralLandUseCode { get; set; }

        public string ParcelId { get; set; }

        public string CadId { get; set; }

        public string TaxId { get; set; }

        public string LegalDescription { get; set; }

        public string LandStateCode { get; set; }

        public string ImprovementStateCode { get; set; }

        public string LandUseCode { get; set; }

        public AddressModel Address { get; set; }

        public bool? SurvivingSpouse { get; set; }

        public bool? DisabilityExemption { get; set; }

        public bool? Mortgage { get; set; }

        public bool? PaymentPlan { get; set; }

        public bool? Veteran { get; set; }

        public bool? Bankruptcy { get; set; }

        public bool? ThirdPartyForeclosure { get; set; }

        public IEnumerable<ContactModel> Contacts { get; set; }

        public IEnumerable<ValuationModel> Valuations { get; set; }

        public IEnumerable<TaxDelinquencyModel> TaxDelinquencies { get; set; }
    }
}